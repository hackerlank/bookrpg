namespace bookrpg.resource
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using bookrpg.log;

    #if  RES_DEBUG
    using UnityEditor;
    #endif

    public class Loader : IDisposable
    {
        private string logTag = "Loader";

        ///e.g. cdn server
        public string baseUrl = "http://localhost/WebPlayer/";
        ///e.g. cdn source server
        public string backupBaseUrl = "http://localhost/WebPlayer/";

        private float startTime;
        private float lastLoadingTime;
        private bool useCache;
        public AssetBundle ab;
        public string actualUrl = string.Empty;
        public object data;
        public string error;
        public bool hasError;
        public bool isDone;
        public int priority;
        public int retryCount;
        public int maxRetryCount;
        private static bool ServerRejectPostRequest = true;
        public int size;
        public string url;
        public int version;
        public WWW www;
        public UnityEngine.Object go;

        ///check ISP redirect or DNS error ...
        public bool checkErrorWebPage = true;


        public float timeout = 7f;
        private int lastBytesLoaded;

        public event Action<Loader> onComplete;

        public bool IsAddDelegate = false;

        public Loader(string url, int version, int maxRetryCount, int priority, int size)
        {
            this.url = url;
            this.version = version;
            this.size = size;
            this.priority = priority;
            this.isDone = false;
            this.maxRetryCount = maxRetryCount;
        }

        ~Loader()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (www != null)
            {
                if (disposing)
                {
                }
                AssetBundleDestroyer.Add(ab);
                www.Dispose();
                www = null;
            }
        }

        public void load(bool useCache, bool useBackupUrl = false)
        {
            #if RES_DEBUG
            go = AssetDatabase.LoadMainAssetAtPath("Assets/" + url);
            #else
            if (www != null)
            {
                throw new InvalidOperationException("Don't reuse Loader");
            }

            useCache = useCache;
            string strUrl;

            if (useBackupUrl)
            {
                strUrl = getActualUrl(url, backupBaseUrl);
                if (strUrl.Contains(baseUrl))
                {
                    strUrl = strUrl.Replace(baseUrl, backupBaseUrl);
                }
            } else
            {
                strUrl = getActualUrl(url, baseUrl);
            }

            actualUrl = strUrl;

            Log.addTagLog(
                logTag, 
                "Load from {0}url: {1}, version: {2}, useCache: {3}", 
                useBackupUrl ? "backup" : "",
                strUrl, 
                version,
                useCache
            );
            www = useCache ? WWW.LoadFromCacheOrDownload(strUrl, version) : new WWW(strUrl);

            startTime = Time.time;
            lastLoadingTime = Time.time;
            lastBytesLoaded = 0f;
            #endif
        }

        private void loadFromBackupUrl(bool useCache = true)
        {
            string strUrl = getActualUrl(url, backupBaseUrl);
            if (strUrl.Contains(baseUrl))
            {
                strUrl = strUrl.Replace(baseUrl, backupBaseUrl);
            }
            actualUrl = strUrl;
            if (useCache)
            {
                www = WWW.LoadFromCacheOrDownload(strUrl);
            } else
            {
                www = new WWW(strUrl);
            }
        }

        private string getActualUrl(string url, string baseUrl)
        {
            string actualUrl;
            //http:// https:// ftp:// file://
            if (url.Contains("://") || string.IsNullOrEmpty(baseUrl))
            {
                actualUrl = url;
            } else
            {
                baseUrl.TrimEnd(new char{ '\\', '/' });
                actualUrl = baseUrl + '/' + url;
            }

            //http or https
            if (actualUrl.StartsWith("http://") || actualUrl.StartsWith("https://"))
            {
                actualUrl += (actualUrl.Contains("?") ? "&loadVer=" : "?loadVer=") + version.ToString();
            }

            return actualUrl;
        }

        public void doCompleted()
        {
            if (onComplete != null)
            {
                onComplete(this);
                onComplete = null;
            }
        }

        /// <summary>
        /// check the load is completed, include load success or failure
        /// </summary>
        public bool checkCompleted()
        {
            if (www == null)
            {
                return true;
            }

            if (www.isDone)
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    string error = www.error;
                    if (retry())
                    {
                        return false;
                    }
                    hasError = true;
                    error = error;
                    return true;
                }

                if (checkErrorWebPage &&
                    (!useCache || (www.assetBundle == null)) &&
                    !string.IsNullOrEmpty(www.text))
                {
                    //ISP redirect to html page, or dns error
                    string str = www.text.Substring(0, Mathf.Min(www.text.Length, 300));
                    if (str.ToLower().Contains("<html"))
                    {
                        if (retry())
                        {
                            return false;
                        }
                        hasError = true;
                        str = www.text.Substring(0, Mathf.Min(www.text.Length, 3000));
                        error = "Load error: " + www.url + "\r\n" + str;
                        return true;
                    }
                }
                return true;
            }

            //is loading
            if (www.bytesDownloaded != lastBytesLoaded)
            {
                lastBytesLoaded = www.bytesDownloaded;
                lastLoadingTime = Time.time;
                return false;
            }

            //waiting or timeout and retry
            if (Time.time - lastLoadingTime <= timeout || retry())
            {
                return false;
            }

            hasError = true;
            error = string.Format("Timeout, start: {0}, now: {1}, pass: {2}", 
                startTime, Time.time, Time.time - startTime);
            return true;
        }

        private bool retry()
        {
            if (www != null)
            {
                www.Dispose();
                www = null;
            }
            if (retryCount >= maxRetryCount)
            {
                return false;
            }

            retryCount++;
            Debug.LogWarning(string.Format("retry load, retryCount: {0}, url: {1}", retryCount, url));
            load(useCache, maxRetryCount - retryCount <= 2);
            return true;
        }

        /// <summary>
        /// return 0 ... 1
        /// </summary>
        public float progress
        {
            get {
                //error or not started
                if (hasError || www == null)
                {
                    return 0f;
                }

                //sucess
                if (isDone)
                {
                    return 1f;
                }

                return size > 0 ? Math.Min(1f, www.bytesDownloaded / size) : www.progress;
            }
        }

        public int bytesLoaded
        {
            get {
                //error or not started
                if (hasError || www == null)
                {
                    return 0f;
                }

                //sucess
                if (isDone)
                {
                    return size > 0 ? size : www.bytesDownloaded;
                }
                
                return www.bytesDownloaded;
            }
        }

        public int bytesTotal
        {
            get {
                if (size > 0)
                {
                    return size;
                }

                return www != null ? www.bytesDownloaded / www.progress : 0f;
            }
        }

        public static float getLoadingProgress(IEnumerable<Loader> list)
        {
            int num;
            int num2;
            float num3;
            getLoadingProgress(list, out num, out num2, out num3);
            return num3;
        }

        public static void getLoadingProgress(
            IEnumerable<Loader> list, 
            out int bytesLoaded, 
            out int bytesTotal, 
            out float progess)
        {
            int loaded = 0;
            int total = 0;
            bool flag = true;
            IEnumerator<Loader> enumerator = list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Loader current = enumerator.Current;
                    //只计算提前知道大小的，便于统计总百分比
                    if (current.size > 0)
                    {
                        total += current.size;
                        loaded += current.bytesLoaded;
                    }
                    flag = flag && current.isDone;
                }
            } finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            bytesLoaded = loaded;
            bytesTotal = total;
            progess = !flag ? ((total != 0) ? (((float)bytesLoaded) / ((float)bytesTotal)) : 0f) : 1f;
        }

        public static bool IsAllDone(IEnumerable<Loader> list)
        {
            IEnumerator<Loader> enumerator = list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Loader current = enumerator.Current;
                    if (!current.isDone)
                    {
                        return false;
                    }
                }
            } finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return true;
        }
    }
}

