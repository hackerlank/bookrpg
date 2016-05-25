using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using bookrpg.mgr;
using bookrpg.log;
using bookrpg.core;
using bookrpg.utils;
using UnityEngine.Events;

namespace bookrpg.resource
{
    public static class LoaderMgr
    {
        ///decide using cache or not, 
        ///BKFunc<string url, bool return>
        public static BKFunc<string, bool> decideUseCache;

        private static string _baseUrl;

        private static string _backupBaseUrl;

        /// <summary>
        /// The max loading count, default 5.
        /// </summary>
        public static int maxLoadingCount = 5;

        public static bool isCheckRedirectError = false;

        /// <summary>
        /// The timeout, default 7s.
        /// </summary>
        public static float timeout = 7f;
        public static string lastError = null;
        public static string lastErrorUrl = null;
        public static int totalLoadedSize = 0;
        public static int totalLoadedCount = 0;

        private static bool needSort;

        private static Dictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();
        private static List<BatchLoader> batchLoaders = new List<BatchLoader>();
        private static List<Loader> waiting = new List<Loader>();
        private static List<Loader> loading = new List<Loader>();
        private static List<Loader> dispatchComplete = new List<Loader>();
        private static Dictionary<string, int> refList = new Dictionary<string, int>();


        public static Loader Load(
            string url, 
            int version = 0, 
            int size = 0,
            int priority = 0, 
            int maxRetryCount = 3)
        {
            return NewOrGetLoad(url, version, size, priority, maxRetryCount);
        }

        private static int CompareLoader(Loader left, Loader right)
        {
            return right.priority - left.priority;
        }

        public static BatchLoader LoadBatch()
        {
            var bl = new BatchLoader();
            batchLoaders.Add(bl);
            return bl;
        }

        public static BatchLoader LoadBatch(ICollection<string> urls, int maxRetryCount = 3)
        {
            var bl = new BatchLoader(urls, maxRetryCount);
            batchLoaders.Add(bl);
            return bl;
        }

        ///like html's baseUrl, e.g. cdn server
        public static string baseUrl
        {
            get{ return _baseUrl; } 
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _baseUrl = value;
                } else
                {
                    _baseUrl = WWW.UnEscapeURL(value).Replace('\\', '/').
                        TrimEnd(new char[]{ '/' }) + "/";
                }
            }
        }

        ///when baseUrl load error, try this, e.g. cdn source server
        public static string backupBaseUrl
        {
            get{ return _backupBaseUrl; } 
            set
            { 
                if (string.IsNullOrEmpty(value))
                {
                    _backupBaseUrl = value;
                } else
                {
                    _backupBaseUrl = WWW.UnEscapeURL(value).Replace('\\', '/').
                        TrimEnd(new char[]{ '/' }) + "/";
                }
            }
        }

        public static bool HasLoaded(string url, int version = 0)
        {
            string key = GetKey(url, version);
            var item = GetCache(key);
            return item.isComplete;
        }

        public static Loader GetLoaded(string url, int version = 0)
        {
            string key = GetKey(url, version);
            var item = GetCache(key);
            return item.isComplete ? item : null;
        }

        public static void StopLoad(string url, int version = 0)
        {
            string key = GetKey(url, version);
            Loader item = null;

            if (!refList.ContainsKey(key) || refList[key] <= 1)
            {
                item = FindLoader(key, loading);
                if (item != null)
                {
                    loading.Remove(item);
                    item.DisposeImmediate();
                }

                item = FindLoader(key, waiting);
                if (item != null)
                {
                    waiting.Remove(item);
                    item.DisposeImmediate();
                }
            }
        }

        public static bool TryUnload(string url, int version = 0)
        {
            string key = GetKey(url, version);
            Loader item = GetCache(key);
            ReleaseRef(key);

            if (item == null || !refList.ContainsKey(key) || refList[key] <= 0)
            {
                Unload(key);
                return true;
            }

            return false;
        }

        //        public static void Unload(string url, int version = 0)
        //        {
        //            Unload(GetKey(url, version));
        //        }

        private static void Unload(string key)
        {
            Loader item;

            item = FindLoader(key, loading);
            if (item != null)
            {
                loading.Remove(item);
                item.DisposeImmediate();
            }

            item = FindLoader(key, waiting);
            if (item != null)
            {
                waiting.Remove(item);
                item.DisposeImmediate();
            }

            item = GetCache(key);
            if (item != null)
            {
                cache.Remove(key);
                item.DisposeImmediate();
            }

            if (refList.ContainsKey(key))
            {
                refList.Remove(key);
            }
        }

        public static void UnloadAll()
        {
            foreach (var item in loading)
            {
                item.DisposeImmediate();
            }

            foreach (var item in waiting)
            {
                item.DisposeImmediate();
            }

            foreach (var weak in cache.Values)
            {
                if (weak.Target != null)
                {
                    Loader item = weak.Target as Loader;
                    item.DisposeImmediate();
                }
            }

            cache.Clear();
            loading.Clear();
            waiting.Clear();
            refList.Clear();
        }

        public static bool CachingAuthorize(string name, string domain, 
                                            long size, int expiration, string singature)
        {
            var isCacheAuthorized = Caching.Authorize(name, domain, size, expiration, singature);
            if (!isCacheAuthorized)
            {
                Debug.LogWarningFormat("Caching.Authorize Failed. name:{0}, domain:{1}, absoluteURL:{2}", 
                    name, domain, Application.absoluteURL);
            } else
            {
                Debug.LogFormat("Caching.Authorize Succeeded. name:{0}, domain:{1}", name, domain);
            }
            return isCacheAuthorized;
        }

        internal static Loader NewOrGetLoad(
            string url, 
            int version = 0, 
            int size = 0, 
            int priority = 0, 
            int maxRetryCount = 3)
        {
            string key = GetKey(url, version);
            Loader target = GetCache(key);
            if (target != null)
            {
                if ((!target.isComplete && !loading.Contains(target)) && !waiting.Contains(target))
                {
                    waiting.Add(target);
                    if (priority > target.priority)
                    {
                        target.priority = priority;
                        needSort = true;
                    }
                    if (maxRetryCount > target.maxRetryCount)
                    {
                        target.maxRetryCount = maxRetryCount;
                    }
                } else
                {
                    dispatchComplete.Add(target);
                }
            } else
            {
                target = new Loader(url, version, size, priority, maxRetryCount);
                waiting.Insert(0, target);
                cache[key] = new WeakReference(target);
                needSort = true;
            }

            AddRef(key);
            return target;
        }

        private static Loader GetCache(string key)
        {
            if (cache.ContainsKey(key))
            {
                WeakReference reference = cache[key];
                if (reference.Target != null)
                {
                    return reference.Target as Loader;
                }
            }
            return null;
        }

        private static void AddRef(string key)
        {
            if (refList.ContainsKey(key))
            {
                refList[key]++;
            } else
            {
                refList.Add(key, 1);
            }
        }

        private static void ReleaseRef(string key)
        {
            if (refList.ContainsKey(key))
            {
                refList[key]--;
            }
        }

        private static Loader FindLoader(string key, IList<Loader> list)
        {
            foreach (var item in list)
            {
                var ikey = GetKey(item.actualUrl != null ? item.actualUrl : item.url, item.version);
                if (ikey == key)
                {
                    return item;
                }
            }

            return null;
        }

        public static List<Loader> DebugGetAllDownloads()
        {
            List<Loader> list = new List<Loader>();
            foreach (KeyValuePair<string, WeakReference> pair in cache)
            {
                Loader target = pair.Value.Target as Loader;
                if (target != null)
                {
                    list.Add(target);
                }
            }
            return list;
        }

        public static void DebugGetDownloadInfo(out int numItems, out int numLoading, out int numWaiting)
        {
            int num = 0;
            foreach (KeyValuePair<string, WeakReference> pair in cache)
            {
                if (pair.Value.Target is Loader)
                {
                    num++;
                }
            }
            numItems = num;
            numLoading = loading.Count;
            numWaiting = waiting.Count;
        }

        public static string GetLoadErrorDetail()
        {
            if (string.IsNullOrEmpty(lastError))
            {
                return string.Empty;
            }
            return string.Format("Load Error, url:{0}\r\nerror:{1}", lastErrorUrl, lastError);
        }

        public static void UpdatePriority()
        {
            needSort = true;
        }

        public static void Update()
        {
            Loader loader;

            foreach (var item in dispatchComplete)
            {
                item.DispatchComplete();
            }
            dispatchComplete.Clear();

            for (int i = 0; i < loading.Count; i++)
            {
                loader = loading[i];
                loader.Update();
                if (!loader.isComplete)
                {
                    continue;
                }

                loading.RemoveAt(i--);
                totalLoadedCount++;

                #if RES_EDITOR
                #else
                if (!loader.hasError)
                {
                    totalLoadedSize += loader.size;
                } else
                {
                    lastErrorUrl = loader.url;
                    lastError = loader.error;
                    Debug.LogErrorFormat("Load Error: {0}, url: {1}, version: {2}, actualUrl:{3}", 
                        loader.error, loader.url, loader.version, loader.actualUrl);
                }

                loader.DispatchComplete();
                #endif
            }

            int count = maxLoadingCount - loading.Count;
            if (count > 0 && waiting.Count > 0)
            {
                if (needSort)
                {
                    needSort = false;
                    waiting.Sort(CompareLoader);
//                    Util.InsertionSort<Loader>(waiting, CompareLoader);
                }
                if (count > waiting.Count)
                {
                    count = waiting.Count;
                }
                for (int i = 0; i < count; i++)
                {
                    loader = waiting[0];
                    waiting.Remove(loader);
                    loading.Add(loader);
                    bool useCache = decideUseCache != null ? decideUseCache(loader.url) : false;
                    loader.Load(useCache);
                }
            }

            for (int i = 0; i < batchLoaders.Count; i++)
            {
                var bl = batchLoaders[i];
                bl.Update();
                if (bl.isComplete || bl.hasDisposed)
                {
                    batchLoaders.RemoveAt(i--);
                }
            }
        }

        private static string GetKey(string url, int version)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            string key = WWW.UnEscapeURL(url);
            key = key.Replace('\\', '/');

            if (!string.IsNullOrEmpty(_baseUrl) && key.Contains(_baseUrl))
            {
                key = key.Replace(_baseUrl, "");
            } else if (!string.IsNullOrEmpty(_backupBaseUrl) &&
                       key.Contains(_backupBaseUrl))
            {
                key = key.Replace(_backupBaseUrl, "");
            }

            key = key.TrimStart(new char[]{ '/' });

            return WWW.UnEscapeURL(key) + '_' + version.ToString();
        }
    }
}