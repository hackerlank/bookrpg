using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{
    public interface IResourceMgr
    {
        Loader Load(string path, BKAction<string> onComplete = null, bool cache = false);

        BatchLoader LoadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false);

        BatchLoader LoadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false);

        bool HasResource(string path);

        bool HasResource(int number);

        UnityEngine.Object GetResource(string path);

        void GetResourceAsync(string path, BKAction<UnityEngine.Object> onComplete);

        T GetResource<T>(string path) where T : UnityEngine.Object;

        void GetResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object[] GetAllResources(string path);

        void GetAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete);

        T[] GetAllResources<T>(string path) where T : UnityEngine.Object;

        void GetAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object GetResource(int number);

        void GetResourceAsync(int number, BKAction<UnityEngine.Object> onComplete);

        T GetResource<T>(int number) where T : UnityEngine.Object;

        void GetResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object;

        UnityEngine.Object[] GetAllResources(int number);

        void GetAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete);

        T[] GetAllResources<T>(int number) where T : UnityEngine.Object;

        void GetAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object;

        /// <summary>
        /// add external loaded resource
        /// </summary>
        void AddResource(Loader loader, bool cache = false);

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        void ReleaseResource(string path);

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        void RemoveResource(string path);

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        void RemoveAllResources();
    }
}
