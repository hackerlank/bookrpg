using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.log;
using bookrpg.core;

namespace bookrpg.resource
{
    public class BatchLoader : IDispose
    {
        public event Action<BatchLoader> onComplete;

        public event Action<Loader> onOneComplete;

        public bool onlyRetainAssetBundle = false;

        ///e.g. cdn server
        private static string _baseUrl;

        ///e.g. cdn source server
        private static string _backupBaseUrl;

        public int maxRetryCount { get; protected set; }

        public bool isCheckRedirectError = false;

        /// <summary>
        /// default value is LoaderMgr.timeout
        /// </summary>
        public float timeout;

        public bool isComplete { get; protected set; }

        ///user's data
        public object customData = null;

        public string lastError  { get; protected set; }

        public string lastErrorUrl  { get; protected set; }

        public float timeElapsed  { get; protected set; }

        /// <summary>
        /// when reach it, stop all load
        /// </summary>
        public int maxErrorCount = 0;

        public int errorCount  { get; protected set; }

        protected float startTime;

        protected Dictionary<string, Loader> loaders = new Dictionary<string, Loader>();
        protected List<Loader> completedLoaders = new List<Loader>();

        protected bool hasInit = false;

        public BatchLoader()
        {
            startTime = Time.time;
            timeout = LoaderMgr.timeout;
            _baseUrl = LoaderMgr.baseUrl;
            _backupBaseUrl = LoaderMgr.backupBaseUrl;
        }

        public BatchLoader(ICollection<string> urls, int maxRetryCount = 3)
        {
            startTime = Time.time;

            foreach (var url in urls)
            {
                addLoader(url, 0, 0, 0, maxRetryCount);
            }
        }

        public Loader addLoader(string url, int version = 0, int size = 0, int priority = 0, int maxRetryCount = 3)
        {
            string key = url + '_' + version.ToString();
            Loader loader;
            if (loaders.ContainsKey(key))
            {
                //for repeated url ignore diff of version
                loader = loaders[key];
                Debug.LogFormat("{0} has already in load queue", url);
            } else
            {
                loader = LoaderMgr.newOrGetLoad(url, version, size, priority, maxRetryCount);
                loaders.Add(key, loader);
            }

            return loader;
        }

        public bool hasDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (hasDisposed)
            {
                return;
            }
            hasDisposed = true;

            onComplete = null;
            onOneComplete = null;
            foreach (var loader in loaders.Values)
            {
                loader.Dispose();
            }
            loaders.Clear();
            completedLoaders.Clear();
            customData = null;
        }

        /// <summary>
        /// Use by LoaderMgr, usually user need't use it
        /// </summary>
        public virtual void disposeImmediate()
        {
            if (hasDisposed)
            {
                return;
            }
            hasDisposed = true;

            onComplete = null;
            onOneComplete = null;
            foreach (var loader in loaders.Values)
            {
                loader.disposeImmediate();
            }
            loaders.Clear();
            completedLoaders.Clear();
            customData = null;
        }

        protected void init()
        {
            foreach (var loader in loaders.Values)
            {
                loader.baseUrl = baseUrl;
                loader.backupBaseUrl = backupBaseUrl;
                loader.onlyRetainAssetBundle = onlyRetainAssetBundle;
                loader.timeout = timeout;
                loader.isCheckRedirectError = isCheckRedirectError;
            }
            hasInit = true;
        }

        ///e.g. cdn server
        public string baseUrl
        { 
            get{ return _baseUrl; } 
            set{ _baseUrl = WWW.UnEscapeURL(value); }
        }

        ///e.g. cdn source server
        public string backupBaseUrl
        {
            get{ return _backupBaseUrl; } 
            set{ _backupBaseUrl = WWW.UnEscapeURL(value); }
        }

        public IDictionary<string, Loader> getLoaders()
        {
            return loaders;
        }

        public Loader getLoader(string url)
        {
            return loaders.ContainsKey(url) ? loaders[url] : null;
        }

        public virtual void update()
        {
            if (isComplete)
            {
                return;
            }

            if (!hasInit)
            {
                init();
            }

            bool isCpt = true;

            foreach (var loader in loaders.Values)
            {
                if (loader.isComplete)
                {
                    if (!completedLoaders.Contains(loader))
                    {
                        if (loader.hasError)
                        {
                            lastError = loader.error;
                            lastErrorUrl = loader.url;
                            errorCount++;
                        }
                        completedLoaders.Add(loader);
                        if (onOneComplete != null)
                        {
                            onOneComplete(loader);
                        }
                    }
                } else
                {
                    isCpt = false;
                }

                if (maxErrorCount > 0 && errorCount > maxErrorCount)
                {
                    LoaderMgr.stopLoad(loader.url, loader.version);
                    isCpt = true;
                }
            }

            if (errorCount > maxErrorCount)
            {
                Debug.LogWarningFormat("BatchLoader stopped, because load error is over maxErrorCount: {0}", 
                    maxErrorCount);
            }

            isComplete = isCpt;

            if (isCpt)
            {
                doCompleted();
            }
        }

        protected void doCompleted()
        {
            timeElapsed = Time.time - startTime;

            if (onComplete != null)
            {
                onComplete(this);
                onComplete = null;
            }

            onOneComplete = null;
        }

        /// <summary>
        /// return 0 ... 1
        /// </summary>
        public float progress
        {
            get
            {
                int bytesLoaded;
                int bytesTotal;
                float progress;
                getLoadingProgress(out bytesLoaded, out bytesTotal, out progress);
                return progress;
            }
        }

        public int bytesLoaded
        {
            get
            {
                int bytesLoaded;
                int bytesTotal;
                float progress;
                getLoadingProgress(out bytesLoaded, out bytesTotal, out progress);
                return bytesLoaded;
            }
        }

        public int bytesTotal
        {
            get
            {
                int bytesLoaded;
                int bytesTotal;
                float progress;
                getLoadingProgress(out bytesLoaded, out bytesTotal, out progress);
                return bytesTotal;
            }
        }

        public void getLoadingProgress(
            out int bytesLoaded, 
            out int bytesTotal, 
            out float progress)
        {
            int loaded = 0;
            int total = 0;
            bool isComplete = true;

            foreach (var loader in loaders.Values)
            {
                //只计算提前知道大小的，便于统计总百分比
                total += loader.bytesTotal;
                loaded += loader.bytesLoaded;
                isComplete = isComplete && loader.isComplete;
            }

            bytesLoaded = loaded;
            bytesTotal = total;
            progress = !isComplete ? (total > 0 ? (float)bytesLoaded / (float)bytesTotal : 0f) : 1f;
        }
    }
}

