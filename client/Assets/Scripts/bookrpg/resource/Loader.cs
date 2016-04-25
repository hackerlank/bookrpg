using System;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.core;
using bookrpg.log;

#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace bookrpg.resource
{
    public class Loader : IDispose
    {
        public static float defaultTimeout = 7f;

        public BKEvent<Loader> onComplete = new BKEvent<Loader>();

        ///e.g. cdn server
        public string baseUrl = "http://localhost/WebPlayer/";
        ///e.g. cdn source server
        public string backupBaseUrl = "http://localhost/WebPlayer/";

        public string actualUrl { get; protected set; }

        public string url { get; protected set; }

        public int version { get; protected set; }

        public int size { get; protected set; }

        public int priority { get; protected set; }

        public int maxRetryCount { get; protected set; }

        public int retryCount { get; protected set; }

        ///check ISP redirect or DNS error ...
        public bool checkErrorWebPage = true;
        public float timeout;

        public string error { get; protected set; }

        public bool isCompete { get; protected set; }

        protected  WWW www;

        public AssetBundle ab;
        ///user's data
        public object data;
        public UnityEngine.Object go;

        protected string logTag = "Loader";
        protected bool useCache;
        protected float startTime = 0f;
        protected float lastLoadingTime;
        protected int lastBytesLoaded;
        protected bool _hasDisposed;
        private ThreadPriority _threadPriority = ThreadPriority.Normal;

        /// <summary>
        /// Why does it has't Loader(void) construction? To prevent reuse Loader instance.
        /// </summary>
        public Loader(string url, int version = 0, int size = 0, int priority = 0, int maxRetryCount = 3)
        {
            this.error = string.Empty;
            this.timeout = defaultTimeout;
            this.actualUrl = string.Empty;
            this.url = url;
            this.version = version;
            this.size = size;
            this.priority = priority;
            this.isCompete = false;
            this.maxRetryCount = maxRetryCount;
        }

        ~Loader()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (_hasDisposed)
            {
                return;
            }

            _hasDisposed = true;
            LoaderMgr.tryUnload(actualUrl, version);
        }

        /// <summary>
        /// Use by LoaderMgr, usual user need't use it
        /// </summary>
        public void disposeImmediate()
        {
            if (www != null)
            {
                www.Dispose();
                www = null;
            }
            _hasDisposed = true;
            GC.SuppressFinalize(this);
        }

        public bool hasDisposed()
        {
            return _hasDisposed;
        }

        public virtual void load(bool useCache = true, bool useBackupUrl = false)
        {
            #if UNITY_EDITOR
            go = AssetDatabase.LoadMainAssetAtPath("Assets/" + url);
            #else
            if (www != null)
            {
                throw new InvalidOperationException("Don't reuse Loader");
            }

            this.useCache = useCache;
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
            www.threadPriority = _threadPriority;

            if (startTime == 0f)
            {
                startTime = Time.time;
            }
            lastLoadingTime = Time.time;
            lastBytesLoaded = 0f;
            isCompete = false;
            #endif
        }

        protected string getActualUrl(string url, string baseUrl)
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

        public virtual void doCompleted()
        {
            if (onComplete != null)
            {
                onComplete.invokeAndRemove(this);
            }
        }

        /// <summary>
        /// check the load is completed, include load success or failure
        /// </summary>
        public virtual bool checkCompleted()
        {
            if (www == null)
            {
                return (isCompete = false);
            }

            if (www.isDone)
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    if (retry())
                    {
                        isCompete = false;
                    } else
                    {
                        error = www.error;
                        isCompete = true;
                    }
                    return isCompete;
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
                            isCompete = false;
                        } else
                        {
                            str = www.text.Substring(0, Mathf.Min(www.text.Length, 3000));
                            error = "Load error: " + www.url + "\r\n" + str;
                            isCompete = true;
                        }

                        return isCompete;
                    }
                }
                error = string.Empty;
                return (isCompete = true);
            }

            //is loading
            if (www.bytesDownloaded != lastBytesLoaded)
            {
                lastBytesLoaded = www.bytesDownloaded;
                lastLoadingTime = Time.time;
                return (isCompete = false);
            }

            //waiting or timeout and retry
            if (Time.time - lastLoadingTime <= timeout || retry())
            {
                return (isCompete = false);
            }

            error = string.Format("Timeout, start: {0}, now: {1}, pass: {2}", 
                startTime, Time.time, Time.time - startTime);
            return (isCompete = true);
        }

        protected virtual bool retry()
        {
            if (retryCount >= maxRetryCount)
            {
                return false;
            }

            if (www != null)
            {
                www.Dispose();
                www = null;
            }

            retryCount++;
            Debug.LogWarning(string.Format("retry load, retryCount: {0}, url: {1}", retryCount, url));
            load(useCache, maxRetryCount - retryCount <= 2);
            return true;
        }


        #region WWW API

        public ResourceBundle assetBundle { get; protected set; }

        public AudioClip audioClip
        { 
            get
            {
                return www == null ? null : www.audioClip;
            }
        }

        public byte[] bytes
        { 
            get
            {
                return www == null ? null : www.bytes;
            }
        }

        public MovieTexture movie
        { 
            get
            {
                return www == null ? null : www.movie;
            }
        }

        public Dictionary<string,string> responseHeaders
        { 
            get
            {
                return www == null ? null : www.responseHeaders;
            }
        }

        public string text
        { 
            get
            {
                return www == null ? null : www.text;
            }
        }

        public Texture2D texture
        { 
            get
            {
                return www == null ? null : www.texture;
            }
        }

        public Texture2D textureNonReadable
        { 
            get
            {
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

        public float progress
        {
            get
            {
                //error or not started
                if (!string.IsNullOrEmpty(error) || www == null)
                {
                    return 0f;
                }
                //sucess
                if (isCompete)
                {
                    return 1f;
                }
//                return size > 0 ? Math.Min(1f, (float)www.bytesDownloaded / (float)size) : www.progress;
                return www.size > 0 ? www.progress : Math.Min(1f, (float)www.bytesDownloaded / (float)size);
            }
        }

        public int bytesLoaded
        {
            get
            {
                //error or not started
                if (!string.IsNullOrEmpty(error) || www == null)
                {
                    return 0;
                }
                //sucess
                if (isCompete)
                {
                    return size > 0 ? size : www.bytesDownloaded;
                }
                return www.bytesDownloaded;
            }
        }

        public int bytesTotal
        {
            get
            {
                if (size > 0)
                {
                    return size;
                }
                return www != null ? www.size : 0;
            }
        }

        public AudioClip getAudioClip(bool threeD)
        {
            return www == null ? null : www.GetAudioClip(threeD);
        }

        public AudioClip getAudioClip(bool threeD, bool stream)
        {
            return www == null ? null : www.GetAudioClip(threeD, stream);
        }

        public AudioClip getAudioClip(bool threeD, bool stream, AudioType audioType)
        {
            return www == null ? null : www.GetAudioClip(threeD, stream, audioType);
        }

        public AudioClip getAudioClipCompressed()
        {
            return www == null ? null : www.GetAudioClipCompressed();
        }

        public AudioClip getAudioClipCompressed(bool threeD)
        {
            return www == null ? null : www.GetAudioClipCompressed(threeD);
        }

        public AudioClip getAudioClipCompressed(bool threeD, AudioType audioType)
        {
            return www == null ? null : www.GetAudioClipCompressed(threeD, audioType);
        }

        public void LoadImageIntoTexture(Texture2D tex)
        {
            if (www != null)
            {
                www.LoadImageIntoTexture(tex);
            }
        }

        #endregion
    }
}
