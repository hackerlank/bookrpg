using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.core;
using bookrpg.log;

#if  RES_EDITOR
using UnityEditor;
#endif

namespace bookrpg.resource
{
    public class Loader : IDispose
    {
        public BKEvent<Loader> onComplete = new BKEvent<Loader>();

        /// <summary>
        /// if the asset is AssetBundle, release the origin bytes
        /// </summary>
        public bool onlyRetainAssetBundle = false;

        ///e.g. cdn server
        public string baseUrl;
        ///e.g. cdn source server
        public string backupBaseUrl;

        public string actualUrl { get; protected set; }

        public string url { get; protected set; }

        public int version { get; protected set; }

        public int size { get; protected set; }

        public int priority { get; set; }

        public int maxRetryCount { get; set; }

        public int retryCount { get; protected set; }

        /// <summary>
        /// Gets the time elapsed of seconds
        /// </summary>
        public float timeElapsed  { get; protected set; }

        ///your resource is not html page, but
        ///ISP redirect or fail DNS was hijacked ...
        public bool isCheckRedirectError = false;

        /// <summary>
        /// default value is LoaderMgr.timeout
        /// </summary>
        public float timeout;

        public string error { get; protected set; }

        public bool isComplete { get; protected set; }

        ///user's data
        public object customData = null;

        #if RES_EDITOR
        public GameObject gameObject = null;
        #endif

        protected  WWW www;
        protected string logTag = "Loader";
        protected bool useCache;
        protected bool isCacheHit;
        protected float startTime = 0f;
        private float lastProgressTime = 0f;
        private float lastProgress = 0;
        private ThreadPriority _threadPriority;

        private int _bytesLoaded;

        protected AssetBundle orgAssetBundle;

        /// <summary>
        /// Why does it has't Loader(void) construction? To prevent reuse Loader instance.
        /// </summary>
        public Loader(string url, int version = 0, int size = 0, int priority = 0, int maxRetryCount = 3)
        {
            this.url = url;
            this.version = version;
            this.size = size;
            this.priority = priority;
            this.maxRetryCount = maxRetryCount;

            this.baseUrl = LoaderMgr.baseUrl;
            this.backupBaseUrl = LoaderMgr.backupBaseUrl;
            this.actualUrl = string.Empty;
            this.isCheckRedirectError = LoaderMgr.isCheckRedirectError;
            this.timeout = LoaderMgr.timeout;
            this.error = string.Empty;
            this.isComplete = false;
            _threadPriority = ThreadPriority.Normal;
        }

        ~Loader()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (hasDisposed)
            {
                return;
            }
            customData = null;
            hasDisposed = true;
            LoaderMgr.TryUnload(actualUrl, version);
        }

        /// <summary>
        /// Use by LoaderMgr, usually user need't use it
        /// </summary>
        public virtual void DisposeImmediate()
        {
            if (www != null)
            {
                if (orgAssetBundle != null)
                {
//                    orgAssetBundle.Unload(false);
                    orgAssetBundle = null;
                    assetBundle.Dispose();
                    assetBundle = null;
                }
                www.Dispose();
                www = null;
            }
            customData = null;
            CoroutineMgr.StopCoroutine("doLoad");
            hasDisposed = true;
            GC.SuppressFinalize(this);
        }

        public bool hasDisposed
        {
            get;
            private set;
        }

        public bool hasError
        {
            get { return !string.IsNullOrEmpty(error); }
        }

        public virtual void Load(bool useCache = true, bool useBackupUrl = false)
        {
            #if RES_EDITOR
            gameObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + url) as GameObject;
            #else
            if (isComplete || www != null || assetBundle != null)
            {
                throw new InvalidOperationException("Don't reuse Loader");
            }

            this.useCache = useCache;
            if (startTime == 0f)
            {
                startTime = Time.time;
            }
            lastProgressTime = Time.time;
            lastProgress = 0f;
            isComplete = false;
            string strUrl;

            if (useBackupUrl)
            {
                strUrl = GetActualUrl(url, backupBaseUrl);
                if (!string.IsNullOrEmpty(baseUrl) &&
                    !string.IsNullOrEmpty(backupBaseUrl) &&
                    strUrl.Contains(baseUrl))
                {
                    strUrl = strUrl.Replace(baseUrl, backupBaseUrl);
                }
            } else
            {
                strUrl = GetActualUrl(url, baseUrl);
            }

            actualUrl = strUrl;

            Log.Debug(logTag, string.Format("{4} load from {0}: {1}, version: {2}, useCache: {3}", 
                useBackupUrl ? "backup url" : "url", strUrl, version, useCache, 
                retryCount > 0 ? retryCount.ToString() + "st retry" : "Start"));

            CoroutineMgr.StartCoroutine(DoLoad(strUrl));
            #endif
        }

        IEnumerator DoLoad(string strUrl)
        {
            www = useCache ? WWW.LoadFromCacheOrDownload(strUrl, version) : new WWW(strUrl);
            www.threadPriority = _threadPriority;
//            isCacheHit = www.isDone && www.assetBundle != null;
//            if (isCacheHit)
//            {
//                Log.AddTagLog(logTag, "Cache hit, url: {0}, version: {1}", strUrl, version);
//            }
            yield return www;

            Update();
        }

        /// <summary>
        /// check the load is completed, include load success or failure
        /// </summary>
        public virtual void Update()
        {
            //not started
            if (www == null || isComplete)
            {
                return;
            }

            //www is done
            if (www.isDone)
            {
                string err = null;
                if (!string.IsNullOrEmpty(www.error))
                {
                    err = www.error;
                } else if (isCheckRedirectError)
                {
                    err = CheckRedirectError();
                }

                if (string.IsNullOrEmpty(err) || !Retry())
                {
                    this.error = err;
                    DoCompleted();
                }
                return;
            } 

            //www is loading
            if (www.progress != lastProgress)
            {
                lastProgress = www.progress;
                lastProgressTime = Time.time;
                return;
            }
           
            //always waiting || waiting for timeout || timeout and retry
            if (timeout <= 0 || Time.time - lastProgressTime <= timeout || Retry())
            {
                return;
            }

            //timeout
            error = string.Format("Timeout, start: {0}, now: {1}", startTime, Time.time);
            DoCompleted();
        }

        ///your resource is not html page, but
        ///ISP redirect or fail DNS was hijacked ...
        protected string CheckRedirectError()
        {
            if (www != null && (!useCache || www.assetBundle == null) &&
                !string.IsNullOrEmpty(www.text))
            {
                var text = www.text;
                if (www.responseHeaders.ContainsKey("CONTENT-TYPE") &&
                    www.responseHeaders["CONTENT-TYPE"].Contains("text/html"))
                {
                    int pos = text.IndexOf("</title>");
                    pos = pos < 0 ? Mathf.Min(text.Length, 2000) : pos + 400;
                    return "resource was illegal redirect: " + text.Substring(0, pos);
                }
//                int pos = www.text.IndexOf("<html");
//                if (pos >= 0 && pos < 300)
//                {
//                    return "resource was illegal redirect: " + www.text.Substring(0, Mathf.Min(www.text.Length, 3000));
//                }
            }

            return string.Empty;
        }

        protected string GetActualUrl(string url, string baseUrl)
        {
            string actualUrl = url;
            //http:// https:// ftp:// file://
            if (!url.Contains("://") && !string.IsNullOrEmpty(baseUrl))
            {
                actualUrl = baseUrl.TrimEnd(new char[]{ '\\', '/' }) + '/' + actualUrl;
            }

            //http or https
            if (version > 0 && (actualUrl.StartsWith("http://") || actualUrl.StartsWith("https://")))
            {
                actualUrl += (actualUrl.Contains("?") ? "&loadVer=" : "?loadVer=") + version.ToString();
            }

            return actualUrl;
        }

        public void DispatchComplete()
        {
            onComplete.InvokeAndRemove(this);
        }

        protected virtual void DoCompleted()
        {
            isComplete = true;

            CoroutineMgr.StopCoroutine("doLoad");

            timeElapsed = Time.time - startTime;

            isCacheHit = www != null && www.size == 0 && www.assetBundle != null;
            if (isCacheHit)
            {
                Log.Debug(logTag, string.Format("Cache hit, url: {0}, version: {1}", actualUrl, version));
            } else
            {
                Log.Debug(logTag, string.Format(
                    "Load complete, url: {3}, version: {2}, time: {0}s, bytesLoaded: {1}, retryCount: {5}, error: {4}", 
                    timeElapsed, bytesLoaded, version, url, !hasError ? "no" : error, retryCount));
            }

            if (!hasError && www != null)
            {
                _bytesLoaded = isCacheHit ? size : www.bytesDownloaded;

                if (www.assetBundle != null)
                {
                    orgAssetBundle = www.assetBundle;
                    assetBundle = new ResourceBundle(orgAssetBundle);
                    //only retain assetBundle, to save memery
                    if (onlyRetainAssetBundle)
                    {
                        www.Dispose();
                        www = null;
                    }
                }

            } else
            {
                _bytesLoaded = 0;
            }

//            onComplete.InvokeAndRemove(this);
        }

        protected virtual bool Retry()
        {
            CoroutineMgr.StopCoroutine("doLoad");

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
//            Debug.LogWarningFormat("Retry load, retryCount: {0}, url: {1}", retryCount, url);
            Load(useCache, maxRetryCount - retryCount < 2);
            return true;
        }


        public AssetBundle GetOrgAssetBundle()
        {
            return orgAssetBundle;
        }

        #region WWW API

        public ResourceBundle assetBundle { get; protected set; }

        public float progress
        {
            get
            {
                return isComplete ? (hasError ? 0f : 1f) : (www != null ? www.progress : 0f);
            }
        }

        public int bytesLoaded
        {
            get
            {
                //when www is loading and access www.bytesDownloaed, it will block thread
                return isComplete ? _bytesLoaded : (int)(www != null ? www.progress * (float)size : 0);
            }
        }

        /// <summary>
        /// when not completed or www.LoadFromCacheOrDownload success, it is 0;
        /// </summary>
        public int bytesTotal
        {
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return size;
                }

                return _bytesLoaded;
            }
        }

        public AudioClip audioClip
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.audioClip;
            }
        }

        public byte[] bytes
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.bytes;
            }
        }

//        not work for mobile
//        public MovieTexture movie
//        { 
//            get
//            {
//                if (!isComplete || isCacheHit)
//                {
//                    return null;
//                }
//                return www == null ? null : www.movie;
//            }
//        }

        public Dictionary<string,string> responseHeaders
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.responseHeaders;
            }
        }

        public string text
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.text;
            }
        }

        public Texture2D texture
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.texture;
            }
        }

        public Texture2D textureNonReadable
        { 
            get
            {
                if (!isComplete || isCacheHit)
                {
                    return null;
                }
                return www == null ? null : www.textureNonReadable;
            }
        }

        public ThreadPriority threadPriority
        { 
            get
            {
                return _threadPriority;
            }
            set
            {
                _threadPriority = value;
                if (www != null)
                {
                    www.threadPriority = value;
                }
            }
        }

        public AudioClip GetAudioClip(bool threeD)
        {
            if (!isComplete || isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClip(threeD);
        }

        public AudioClip GetAudioClip(bool threeD, bool stream)
        {
            if (!isComplete || isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClip(threeD, stream);
        }

        public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
        {
            if (isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClip(threeD, stream, audioType);
        }

        public AudioClip GetAudioClipCompressed()
        {
            if (!isComplete || isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClipCompressed();
        }

        public AudioClip GetAudioClipCompressed(bool threeD)
        {
            if (!isComplete || isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClipCompressed(threeD);
        }

        public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
        {
            if (!isComplete || isCacheHit)
            {
                return null;
            }
            return www == null ? null : www.GetAudioClipCompressed(threeD, audioType);
        }

        public void LoadImageIntoTexture(Texture2D tex)
        {
            if (!isComplete || isCacheHit)
            {
                return;
            }
            if (www != null)
            {
                www.LoadImageIntoTexture(tex);
            }
        }

        #endregion
    }
}
