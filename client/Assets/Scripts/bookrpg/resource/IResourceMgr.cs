using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{
    public interface IResourceMgr
    {
        void init(IResourceTable table);

        Loader load(string path, BKAction<string> onComplete = null, bool cache = false);

        BatchLoader loadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false);

        BatchLoader loadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false);

        bool hasResource(string path);

        bool hasResource(int number);

        UnityEngine.Object getResource(string path);

        void getResourceAsync(string path, BKAction<UnityEngine.Object> onComplete);

        T getResource<T>(string path) where T : UnityEngine.Object;

        void getResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object[] getAllResources(string path);

        void getAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete);

        T[] getAllResources<T>(string path) where T : UnityEngine.Object;

        void getAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object getResource(int number);

        void getResourceAsync(int number, BKAction<UnityEngine.Object> onComplete);

        T getResource<T>(int number) where T : UnityEngine.Object;

        void getResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object[] getAllResources(int number);

        void getAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete);

        T[] getAllResources<T>(int number) where T : UnityEngine.Object;

        void getAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object;

        /// <summary>
        /// add external loaded resource
        /// </summary>
        void addResource(Loader loader, bool cache = false);

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        void releaseResource(string path);

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        void removeResource(string path);

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        void removeAllResources();
    }
}
