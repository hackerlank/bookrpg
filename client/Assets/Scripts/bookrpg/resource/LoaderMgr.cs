using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using bookrpg.log;
using bookrpg.utils;
using UnityEngine.Events;

namespace bookrpg.resource
{
    public static class LoaderMgr
    {
        ///e.g. cdn server
        public static string baseUrl {get; private set;}
        ///e.g. cdn source server
        public static string backupBaseUrl {get; private set;}

        private static string decodedBaseUrl;
        private static string decodedBackupBaseUrl;

        private static Dictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();
        private static List<Loader> waiting = new List<Loader>();
        private static List<Loader> loading = new List<Loader>();
        private static Dictionary<string, int> refList = new Dictionary<string, int>();

        private static int CompareLoader(Loader left, Loader right)
        {
            return left.priority - right.priority;
        }

        public static bool isCacheAuthorized = false;
        public static bool hasError = false;
        public static bool hasItemDone = false;
        public static string LastErrorMsg = null;
        public static string LastErrorUrl = null;
        public static int maxLoading = 5;
        private static bool needSort;
        public static int sizeLoaded = 0;
        public static int totalLoaded = 0;

        public static Loader load(
            string url, 
            int version = 0, 
            int size = 0, 
            int priority = 0, 
            int maxRetryCount = 3)
        {
            return newOrGetLoad(url, version, size, priority, maxRetryCount);

            UnityAction a;
        } 

        public static BatchLoader batchLoad()
        {
            return new BatchLoader();
        }

        public static BatchLoader batchLoad(string[] urls, int maxRetryCount = 3)
        {
            return new BatchLoader(urls, maxRetryCount);
        }

        public static void setBaseUrl(string baseUrl, string backupBaseUrl = "")
        {
            LoaderMgr.baseUrl = baseUrl;
            decodedBaseUrl = WWW.UnEscapeURL(baseUrl);
            LoaderMgr.backupBaseUrl = backupBaseUrl;
            decodedBackupBaseUrl = WWW.UnEscapeURL(backupBaseUrl);
        }

        public static bool hasLoaded(string url, int version = 0)
        {
            string key = url + '_' + version.ToString();
            return cache.ContainsKey(key) && cache[key].Target != null;
        }

        public static Loader getLoaded(string url, int version = 0)
        {
            string key = url + '_' + version.ToString();
            return cache.ContainsKey(key) ? cache[key].Target as Loader : null;
        }



        public static bool tryUnload(string url, int version = 0)
        {
            Loader item = null;
            string key = getKey(url, version);

            if (cache.ContainsKey(key) && cache[key].Target != null)
            {
                item = cache[key].Target as Loader;
            }

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

            if (cache.ContainsKey(key) && cache[key].Target != null)
            {
                item = cache[key].Target as Loader;
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
            isCacheAuthorized = Caching.Authorize(name, domain, size, expiration, singature);
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
            Loader target = null;
            if (cache.ContainsKey(key))
            {
                WeakReference reference = cache[key];
                target = reference.Target as Loader;
                if (target != null)
                {
                    if ((!target.isCompete && !loading.Contains(target)) && !waiting.Contains(target))
                    {
                        waiting.Add(target);
                        needSort = true;
                    }
                    return target;
                }
            }
            target = new Loader(url, version, size, priority, maxRetryCount);
            waiting.Insert(0, target);
            needSort = true;
            cache[key] = new WeakReference(target);
            return target;
        }

        private static Loader findLoader(string key, IList<Loader> list)
        {
            foreach (var item in list)
            {
                var ikey = getKey(item.actualUrl != null ? item.actualUrl : item.url, item.version);
                if (ikey == key){
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
            return (!hasError ? string.Empty : string.Format("Load Error, url:{0}\nmsg:{1}", LastErrorUrl, LastErrorMsg));
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

        public static void OnPriorityChanged()
        {
            needSort = true;
        }

        public static void update()
        {

            for (int i = 0; i < loading.Count; i++)
            {
                var loader = loading[i];
                if (!loader.checkCompleted())
                {
                    continue;
                }

                loading.Remove(loader);
                i--;
                totalLoaded++;

                #if UNITY_EDITOR1
                #else
                if (!string.IsNullOrEmpty(loader.error))
                {
//                   TODO Log.addTagLog
                    sizeLoaded += loader.size;
                } else
                {
                    hasError = true;
                    LastErrorUrl = loader.url;
                    LastErrorMsg = loader.error;
                    Debug.LogErrorFormat("Load Error: {0}, url: {1}, version: {2}, actualUrl:{3}", 
                        loader.error, loader.url, loader.version, loader.actualUrl);
                }
                #endif

                loader.doCompleted();
            }

            int count = maxLoading - loading.Count;
            if (count > 0 && waiting.Count > 0)
            {
                if (needSort)
                {
                    needSort = false;
//                    waiting.Sort(CompareLoader);
                    Util.insertionSort<Loader>(waiting, CompareLoader);
                }
                if (count > waiting.Count)
                {
                    count = waiting.Count;
                }
                for (int i = 0; i < count; i++)
                {
                    Loader item = waiting[0];
                    waiting.Remove(item);
                    loading.Add(item);
//                    bool useCache = CacheAuthorized && IsAssetBundle(item3.url);
                    item.load(true);
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

            if (string.IsNullOrEmpty(decodedBaseUrl) && key.Contains(decodedBaseUrl))
            {
                key = key.Replace(decodedBaseUrl, "");
            } else if (string.IsNullOrEmpty(decodedBackupBaseUrl) && 
                key.Contains(decodedBackupBaseUrl))
            {
                key = key.Replace(decodedBackupBaseUrl, "");
            }

            key = key.Replace('\\', '/');
            key = key.TrimStart(new char[]{'/'});

            return WWW.UnEscapeURL(key) + '_' + version.ToString();
        }
    }
}