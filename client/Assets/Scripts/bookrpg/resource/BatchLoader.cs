using System;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.log;

namespace bookrpg.resource
{
    public class BatchLoader : IDisposable
    {
        public static int defaultMaxLoadingCount = 3;
        public event Action<BatchLoader> onComplete;

        ///e.g. cdn server
        public string baseUrl = "http://localhost/WebPlayer/";
        ///e.g. cdn source server
        public string backupBaseUrl = "http://localhost/WebPlayer/";

        public int maxRetryCount { get; protected set; }

        public int maxLoadingCount;

        ///check ISP redirect or DNS error ...
        public bool checkErrorWebPage = true;
        public float timeout;

        public string error { get; protected set; }

        public bool isCompete { get; protected set; }

        ///user's data
        public object data;

        protected bool useCache;
        protected float startTime;

        protected Dictionary<string, Loader> loaders = new Dictionary<string, Loader>();

        public BatchLoader()
        {
            
        }

        public BatchLoader(string[] urls, int maxRetryCount = 3)
        {
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
                Debug.LogFormat("%s has already in load queue", url);
            } else
            {
                loader = LoaderMgr.newOrGetLoad(url, version, size, priority, maxRetryCount);
                loaders.Add(key, loader);
            }

            return loader;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual void load(bool useCache = true, bool useBackupUrl = false)
        {
            
        }

       
        public virtual void doCompleted()
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
        public virtual bool checkCompleted()
        {
            foreach (var loader in loaders.Values)
            {
                if (!loader.isCompete)
                {
                    return false;
                }
            }

            return true;
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

        protected void getLoadingProgress(
            out int bytesLoaded, 
            out int bytesTotal, 
            out float progress)
        {
            int loaded = 0;
            int total = 0;
            bool isCompete = true;

            foreach (var loader in loaders.Values)
            {
                //只计算提前知道大小的，便于统计总百分比
                if (loader.size > 0)
                {
                    total += loader.size;
                    loaded += loader.bytesLoaded;
                }
                isCompete = isCompete && loader.isCompete;
            }

            bytesLoaded = loaded;
            bytesTotal = total;
            progress = !isCompete ? (total > 0 ? (float)bytesLoaded / (float)bytesTotal : 0f) : 1f;
        }
    }
}

