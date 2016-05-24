using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{
    public class ResourceMgr
    {
        private static IResourceMgr impl = new ResourceMgrImpl();

        public static void Init(IResourceMgr instance)
        {
            impl = instance;
        }

        public static Loader Load(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return impl.Load(path, onComplete, cache);
        }

        public static BatchLoader LoadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            return impl.LoadWithDependencies(path, onComplete, cache);
        }

        public static BatchLoader LoadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false)
        {
            return impl.LoadBatch(pathes, onComplete, cache);
        }

        public static bool HasResource(string path)
        {
            return impl.HasResource(path);
        }

        public static bool HasResource(int number)
        {
            return impl.HasResource(number);
        }

        public static UnityEngine.Object GetResource(string path)
        {
            return impl.GetResource(path);
        }

        public static void GetResourceAsync(string path, BKAction<UnityEngine.Object> onComplete)
        {
            impl.GetResourceAsync(path, onComplete);
        }

        public static T GetResource<T>(string path) where T : UnityEngine.Object
        {
            return impl.GetResource<T>(path);
        }

        public static void GetResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            impl.GetResourceAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object[] GetAllResources(string path)
        {
            return impl.GetAllResources(path);
        }

        public static void GetAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete)
        {
            impl.GetAllResourcesAsync(path, onComplete);
        }

        public static T[] GetAllResources<T>(string path) where T : UnityEngine.Object
        {
            return impl.GetAllResources<T>(path);
        }

        public static void GetAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            impl.GetAllResourcesAsync<T>(path, onComplete);
        }

        public static UnityEngine.Object GetResource(int number)
        {
            return impl.GetResource(number);
        }

        public static void GetResourceAsync(int number, BKAction<UnityEngine.Object> onComplete)
        {
            impl.GetResourceAsync(number, onComplete);
        }

        public static T GetResource<T>(int number) where T : UnityEngine.Object
        {
            return impl.GetResource<T>(number);
        }

        public static void GetResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            impl.GetResourceAsync<T>(number, onComplete);
        }

        public static UnityEngine.Object[] GetAllResources(int number)
        {
            return impl.GetAllResources(number);
        }

        public static void GetAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete)
        {
            impl.GetAllResourcesAsync(number, onComplete);
        }

        public static T[] GetAllResources<T>(int number) where T : UnityEngine.Object
        {
            return impl.GetAllResources<T>(number);
        }

        public static void GetAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            impl.GetAllResourcesAsync<T>(number, onComplete);
        }

        /// <summary>
        /// add external loaded resource
        /// </summary>
        public static void AddResource(Loader loader, bool cache = false)
        {
            impl.AddResource(loader, cache);
        }

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        public static void ReleaseResource(string path)
        {
            impl.ReleaseResource(path);
        }

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        public static void RemoveResource(string path)
        {
            impl.RemoveResource(path);
        }

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        public static void RemoveAllResources()
        {
            impl.RemoveAllResources();
        }
    }
}
