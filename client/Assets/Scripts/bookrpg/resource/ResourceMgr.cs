using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{
    public class ResourceMgr
    {
        private static IResourceMgr impl = new ResourceMgrImpl();

        public static void init(IResourceMgr instance)
        {
            impl = instance;
        }

        public static Loader load(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return impl.load(path, onComplete, cache);
        }

        public static BatchLoader loadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return impl.loadWithDependencies(path, onComplete, cache);
        }

        public static BatchLoader loadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false)
        {
            return impl.loadBatch(pathes, onComplete, cache);
        }

        public static bool hasResource(string path)
        {
            return impl.hasResource(path);
        }

        public static bool hasResource(int number)
        {
            return impl.hasResource(number);
        }

        public static UnityEngine.Object getResource(string path)
        {
            return impl.getResource(path);
        }

        public static void getResourceAsync(string path, BKAction<UnityEngine.Object> onComplete)
        {
            impl.getResourceAsync(path, onComplete);
        }

        public static T getResource<T>(string path) where T : UnityEngine.Object
        {
            return impl.getResource<T>(path);
        }

        public static void getResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            impl.getResourceAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object[] getAllResources(string path)
        {
            return impl.getAllResources(path);
        }

        public static void getAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete)
        {
            impl.getAllResourcesAsync(path, onComplete);
        }

        public static T[] getAllResources<T>(string path) where T : UnityEngine.Object
        {
            return impl.getAllResources<T>(path);
        }

        public static void getAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            impl.getAllResourcesAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object getResource(int number)
        {
            return impl.getResource(number);
        }

        public static void getResourceAsync(int number, BKAction<UnityEngine.Object> onComplete)
        {
            impl.getResourceAsync(number, onComplete);
        }

        public static T getResource<T>(int number) where T : UnityEngine.Object
        {
            return impl.getResource<T>(number);
        }

        public static void getResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            impl.getResourceAsync<T>(number, onComplete);
        }

        public static UnityEngine.Object[] getAllResources(int number)
        {
            return impl.getAllResources(number);
        }

        public static void getAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete)
        {
            impl.getAllResourcesAsync(number, onComplete);
        }

        public static T[] getAllResources<T>(int number) where T : UnityEngine.Object
        {
            return impl.getAllResources<T>(number);
        }

        public static void getAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            impl.getAllResourcesAsync<T>(number, onComplete);
        }

        /// <summary>
        /// add external loaded resource
        /// </summary>
        public static void addResource(Loader loader, bool cache = false)
        {
            impl.addResource(loader, cache);
        }

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        public static void releaseResource(string path)
        {
            impl.releaseResource(path);
        }

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        public static void removeResource(string path)
        {
            impl.removeResource(path);
        }

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        public static void removeAllResources()
        {
            impl.removeAllResources();
        }
    }
}
