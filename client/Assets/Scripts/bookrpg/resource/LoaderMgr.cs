using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
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


        public static Loader load(
            string url, 
            int version = 0, 
            int size = 0,
            int priority = 0, 
            int maxRetryCount = 3)
        {
            return newOrGetLoad(url, version, size, priority, maxRetryCount);
        }

        private static int compareLoader(Loader left, Loader right)
        {
            return right.priority - left.priority;
        }

        public static BatchLoader loadBatch()
        {
            var bl = new BatchLoader();
            batchLoaders.Add(bl);
            return bl;
        }

        public static BatchLoader loadBatch(ICollection<string> urls, int maxRetryCount = 3)
        {
            var bl = new BatchLoader(urls, maxRetryCount);
            batchLoaders.Add(bl);
            return bl;
        }


        ///like html's baseUrl, e.g. cdn server
        public static string baseUrl
        {
            get{ return _baseUrl; } 
            set{ _baseUrl = WWW.UnEscapeURL(value); }
        }

        ///when baseUrl load error, try this, e.g. cdn source server
        public static string backupBaseUrl
        {
            get{ return _backupBaseUrl; } 
            set{ _backupBaseUrl = WWW.UnEscapeURL(value); }
        }

        public static bool hasLoaded(string url, int version = 0)
        {
            string key = getKey(url, version);
            var item = getCache(key);
            return item.isComplete;
        }

        public static Loader getLoaded(string url, int version = 0)
        {
            string key = getKey(url, version);
            var item = getCache(key);
            return item.isComplete ? item : null;
        }

        public static void stopLoad(string url, int version = 0)
        {
            string key = getKey(url, version);
            Loader item = null;

            if (!refList.ContainsKey(key) || refList[key] <= 1)
            {
                item = findLoader(key, loading);
                if (item != null)
                {
                    loading.Remove(item);
                    item.disposeImmediate();
                }

                item = findLoader(key, waiting);
                if (item != null)
                {
                    waiting.Remove(item);
                    item.disposeImmediate();
                }
            }
        }

        public static bool tryUnload(string url, int version = 0)
        {
            string key = getKey(url, version);
            Loader item = getCache(key);
            releaseRef(key);

            if (item == null || !refList.ContainsKey(key) || refList[key] <= 0)
            {
                unload(key);
                return true;
            }

            return false;
        }

        //        public static void unload(string url, int version = 0)
        //        {
        //            unload(getKey(url, version));
        //        }

        private static void unload(string key)
        {
            Loader item;

            item = findLoader(key, loading);
            if (item != null)
            {
                loading.Remove(item);
                item.disposeImmediate();
            }

            item = findLoader(key, waiting);
            if (item != null)
            {
                waiting.Remove(item);
                item.disposeImmediate();
            }

            item = getCache(key);
            if (item != null)
            {
                cache.Remove(key);
                item.disposeImmediate();
            }

            if (refList.ContainsKey(key))
            {
                refList.Remove(key);
            }
        }

        public static void unloadAll()
        {
            foreach (var item in loading)
            {
                item.disposeImmediate();
            }

            foreach (var item in waiting)
            {
                item.disposeImmediate();
            }

            foreach (var weak in cache.Values)
            {
                if (weak.Target != null)
                {
                    Loader item = weak.Target as Loader;
                    item.disposeImmediate();
                }
            }

            cache.Clear();
            loading.Clear();
            waiting.Clear();
            refList.Clear();
        }

        public static bool cachingAuthorize(string name, string domain, long size, int expiration, string singature)
        {
            var isCacheAuthorized = Caching.Authorize(name, domain, size, expiration, singature);
            if (!isCacheAuthorized)
            {
                Debug.LogWarningFormat("Caching.Authorize Failed. name:{0}, domain:{1}, absoluteURL:{2}", name, domain, Application.absoluteURL);
            } else
            {
                Debug.LogFormat("Caching.Authorize Succeeded. name:{0}, domain:{1}", name, domain);
            }
            return isCacheAuthorized;
        }

        internal static Loader newOrGetLoad(
            string url, 
            int version = 0, 
            int size = 0, 
            int priority = 0, 
            int maxRetryCount = 3)
        {
            string key = getKey(url, version);
            Loader target = getCache(key);
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

            addRef(key);
            return target;
        }

        private static Loader getCache(string key)
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

        private static void addRef(string key)
        {
            if (refList.ContainsKey(key))
            {
                refList[key]++;
            } else
            {
                refList.Add(key, 1);
            }
        }

        private static void releaseRef(string key)
        {
            if (refList.ContainsKey(key))
            {
                refList[key]--;
            }
        }

        private static Loader findLoader(string key, IList<Loader> list)
        {
            foreach (var item in list)
            {
                var ikey = getKey(item.actualUrl != null ? item.actualUrl : item.url, item.version);
                if (ikey == key)
                {
                    return item;
                }
            }

            return null;
        }

        public static List<Loader> debugGetAllDownloads()
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

        public static void debugGetDownloadInfo(out int numItems, out int numLoading, out int numWaiting)
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

        public static string getLoadErrorDetail()
        {
            if (string.IsNullOrEmpty(lastError))
            {
                return string.Empty;
            }
            return string.Format("Load Error, url:{0}\r\nerror:{1}", lastErrorUrl, lastError);
        }

        public static void init(bool autoUpdate = true)
        {   
            if (autoUpdate)
            {
                CoroutineMgr.startCoroutine(Loop());
            }
        }

        public static IEnumerator Loop()
        {
            while (true)
            {
                update();
                yield return 0;
            }
        }

        public static void onPriorityChanged()
        {
            needSort = true;
        }

        public static void update()
        {
            Loader loader;

            foreach(var item in dispatchComplete)
            {
                item.dispatchComplete();
            }
            dispatchComplete.Clear();

            for (int i = 0; i < loading.Count; i++)
            {
                loader = loading[i];
                loader.update();
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

                loader.dispatchComplete();
                #endif
            }

            int count = maxLoadingCount - loading.Count;
            if (count > 0 && waiting.Count > 0)
            {
                if (needSort)
                {
                    needSort = false;
                    waiting.Sort(compareLoader);
//                    Util.insertionSort<Loader>(waiting, compareLoader);
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
                    loader.load(useCache);
                }
            }

            for (int i = 0; i < batchLoaders.Count; i++)
            {
                var bl = batchLoaders[i];
                bl.update();
                if (bl.isComplete || bl.hasDisposed)
                {
                    batchLoaders.RemoveAt(i--);
                }
            }
        }

        private static string getKey(string url, int version)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            string key = WWW.UnEscapeURL(url);

            if (!string.IsNullOrEmpty(_baseUrl) && key.Contains(_baseUrl))
            {
                key = key.Replace(_baseUrl, "");
            } else if (!string.IsNullOrEmpty(_backupBaseUrl) &&
                       key.Contains(_backupBaseUrl))
            {
                key = key.Replace(_backupBaseUrl, "");
            }

            key = key.Replace('\\', '/');
            key = key.TrimStart(new char[]{ '/' });

            return WWW.UnEscapeURL(key) + '_' + version.ToString();
        }
    }
}