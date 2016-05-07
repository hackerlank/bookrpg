using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{
    public class ResourceMgr
    {
        private static IResourceMgr instance;

        public static void init(IResourceMgr ins)
        {
            instance = ins;
        }

        public static Loader load(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return instance.load(path, onComplete, cache);
        }

        public static BatchLoader loadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return instance.loadWithDependencies(path, onComplete, cache);
        }

        public static BatchLoader loadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false)
        {
            return instance.loadBatch(pathes, onComplete, cache);
        }

        public static bool hasResource(string path)
        {
            return instance.hasResource(path);
        }

        public static bool hasResource(int number)
        {
            return instance.hasResource(number);
        }

        public static UnityEngine.Object getResource(string path)
        {
            return instance.getResource(path);
        }

        public static void getResourceAsync(string path, BKAction<UnityEngine.Object> onComplete)
        {
            instance.getResourceAsync(path, onComplete);
        }

        public static T getResource<T>(string path) where T : UnityEngine.Object
        {
            return instance.getResource<T>(path);
        }

        public static void getResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            instance.getResourceAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object[] getAllResources(string path)
        {
            return instance.getAllResources(path);
        }

        public static void getAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete)
        {
            instance.getAllResourcesAsync(path, onComplete);
        }

        public static T[] getAllResources<T>(string path) where T : UnityEngine.Object
        {
            return instance.getAllResources<T>(path);
        }

        public static void getAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            instance.getAllResourcesAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object getResource(int number)
        {
            return instance.getResource(number);
        }

        public static void getResourceAsync(int number, BKAction<UnityEngine.Object> onComplete)
        {
            instance.getResourceAsync(number, onComplete);
        }

        public static T getResource<T>(int number) where T : UnityEngine.Object
        {
            return instance.getResource<T>(number);
        }

        public static void getResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            instance.getResourceAsync<T>(number, onComplete);
        }

        public static UnityEngine.Object[] getAllResources(int number)
        {
            return instance.getAllResources(number);
        }

        public static void getAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete)
        {
            instance.getAllResourcesAsync(number, onComplete);
        }

        public static T[] getAllResources<T>(int number) where T : UnityEngine.Object
        {
            return instance.getAllResources<T>(number);
        }

        public static void getAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            instance.getAllResourcesAsync<T>(number, onComplete);
        }

        /// <summary>
        /// add external loaded resource
        /// </summary>
        public static void addResource(Loader loader, bool cache = false)
        {
            instance.addResource(loader, cache);
        }

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        public static void releaseResource(string path)
        {
            instance.releaseResource(path);
        }

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        public static void removeResource(string path)
        {
            instance.removeResource(path);
        }

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        public static void removeAllResources()
        {
            instance.removeAllResources();
        }
    }
}
