using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{

    public class ResourceMgrImpl : IResourceMgr
    {
        private IResourceTable resourceTable;

        private Dictionary<string, CountableRef> assetList = new Dictionary<string, CountableRef>();
        private Dictionary<string, CountableRef> assetBundleList = new Dictionary<string, CountableRef>();

        public ResourceMgrImpl()
        {
            resourceTable = new ResourceTableImpl();
        }

        public ResourceMgrImpl(IResourceTable table)
        {
            resourceTable = table;
        }

        public Loader Load(string path, BKAction<string> onComplete = null, 
            bool cache = false)
        {
            var bReturn = false;
            if (string.IsNullOrEmpty(path))
            {
                bReturn = true;
                Debug.LogWarning("LocalResMgr.load, path is empty");
            }
            if (HasResource(path))
            {
                bReturn = true;
                UpdateCache(path, cache);
            }
            if (bReturn)
            {
                if (onComplete != null)
                {
                    onComplete(path);
                }
                return null;
            }

            var file = resourceTable.GetResourceFile(path);
            var loader = file == null ? LoaderMgr.Load(path) : 
                LoaderMgr.Load(file.targetFile, file.version, file.size);

            loader.onComplete += ld =>
            {
                if (!ld.hasError)
                {
                    AddResource(path, ld.GetOrgAssetBundle(), cache);
                }

                if (onComplete != null)
                {
                    onComplete(path);
                }

                ld.Dispose();
            };

            return loader;
        }

        public BatchLoader LoadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            if (HasResource(path))
            {
                return null;
            }

            var batch = LoaderMgr.LoadBatch();
            var file = resourceTable.GetResourceFile(path);
            if (file == null)
            {
                batch.AddLoader(path);
            } else
            {
                if (file.dependencies != null)
                {
                    foreach (var filestr in file.dependencies)
                    {
                        var file2 = resourceTable.GetResourceFile(filestr);
                        if (!HasResource(file2))
                        {
                            batch.AddLoader(file2.targetFile, file2.version, file2.size);
                        }
                    }
                }

                batch.AddLoader(file.targetFile, file.version, file.size);
            }

            if (batch.GetLoaders().Count == 0)
            {
                batch.DisposeImmediate();
                if (onComplete != null)
                {
                    onComplete(path);
                }
                return null;
            }

            batch.onComplete += bt =>
            {
                foreach (var item in bt.GetLoaders())
                {
                    if (!item.Value.hasError)
                    {
                        AddResource(item.Key, item.Value.GetOrgAssetBundle(), cache);
                    }
                }

                if (onComplete != null)
                {
                    onComplete(path);
                }

                bt.Dispose();
            };

            return batch;
        }

        public BatchLoader LoadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false)
        {
            var batch = LoaderMgr.LoadBatch();
            
            foreach (var path in pathes)
            {
                if (HasResource(path))
                {
                    continue;
                }

                var file = resourceTable.GetResourceFile(path);
                if (file == null)
                {
                    batch.AddLoader(path);
                } else
                {
                    if (file.dependencies != null)
                    {
                        foreach (var filestr in file.dependencies)
                        {
                            var file2 = resourceTable.GetResourceFile(filestr);
                            if (!HasResource(file2))
                            {
                                batch.AddLoader(file2.targetFile, file2.version, file2.size);
                            }
                        }
                    }

                    batch.AddLoader(file.targetFile, file.version, file.size);
                }
            }

            if (batch.GetLoaders().Count == 0)
            {
                batch.DisposeImmediate();
                if (onComplete != null)
                {
                    onComplete(pathes);
                }
                return null;
            }

            batch.onComplete += bt =>
            {
                foreach (var item in bt.GetLoaders())
                {
                    if (!item.Value.hasError)
                    {
                        AddResource(item.Key, item.Value.GetOrgAssetBundle(), cache);
                    }
                }

                if (onComplete != null)
                {
                    onComplete(pathes);
                }

                bt.Dispose();
            };

            return batch;
        }

        public bool HasResource(string path)
        {
            //后者表示资源就是AssetBundle包本身，而不是包中的文件
            if (assetList.ContainsKey(path) ||
                (assetBundleList.ContainsKey(path) &&
                assetBundleList[path].target != null))
            {
                return true;
            }

            var file = resourceTable.GetResourceFile(path);
            return file != null && HasResource(file);
        }

        public bool HasResource(int number)
        {
            var file = resourceTable.GetResourceFile(number);
            return file != null && HasResource(file);
        }

        #region getResource by path

        public UnityEngine.Object GetResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                return assetList[path].RefTarget() as UnityEngine.Object;
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                return null;
            }

            IResourceFile file = resourceTable.GetResourceFile(path);
            var obj = file == null ? ab : ab.LoadAsset(file.idInPack, GetResourceType(file.type));
            BundleToAsset(path, file, ab, obj);
            return obj;
        }

        public void GetResourceAsync(string path, BKAction<UnityEngine.Object> onComplete)
        {
            if (assetList.ContainsKey(path))
            {
                onComplete((assetList[path].RefTarget() as UnityEngine.Object));
                return;
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                onComplete(null);
                return;
            }

            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file == null)
            {
                BundleToAsset(path, file, ab, ab);
                onComplete(ab);
                return;
            }

            CoroutineMgr.StartCoroutine(DoGetResourceAsync(file, ab, onComplete));
        }

        public T GetResource<T>(string path) where T : UnityEngine.Object
        {
            if (assetList.ContainsKey(path))
            {
                return (T)(assetList[path].RefTarget());
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                return default(T);
            }

            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file == null)
            {
                BundleToAsset(path, file, ab, ab);
                return (T)Convert.ChangeType(ab, typeof(T));
            } else
            {
                var obj = ab.LoadAsset<T>(file.idInPack);
                BundleToAsset(path, file, ab, obj);
                return obj;
            }
        }

        public void GetResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            if (assetList.ContainsKey(path))
            {
                onComplete((T)(assetList[path].RefTarget()));
                return;
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                onComplete(default(T));
                return;
            }

            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file == null)
            {
                BundleToAsset(path, file, ab, ab);
                onComplete((T)Convert.ChangeType(ab, typeof(T)));
                return;
            }

            CoroutineMgr.StartCoroutine(DoGetResourceAsync<T>(file, ab, onComplete));
        }

        #endregion

        #region getAllResources

        public UnityEngine.Object[] GetAllResources(string path)
        {
            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                return new UnityEngine.Object[]{ assetList[file.srcFile].RefTarget() as UnityEngine.Object };
            }

            var ab = GetAssetBundle(path);
            return ab == null ? null : ab.LoadAllAssets();
        }

        public void GetAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete)
        {
            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                onComplete(new UnityEngine.Object[]{ assetList[file.srcFile].RefTarget() as UnityEngine.Object });
                return;
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                return;
            }

            CoroutineMgr.StartCoroutine(DoGetAllResourcesAsync<UnityEngine.Object>(ab, onComplete));
        }

        public T[] GetAllResources<T>(string path) where T : UnityEngine.Object
        {
            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                return new T[]{ (T)assetList[file.srcFile].RefTarget() };
            }

            var ab = GetAssetBundle(path);
            return ab == null ? null : (T[])ab.LoadAllAssets();
        }

        public void GetAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {

            IResourceFile file = resourceTable.GetResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                onComplete(new T[]{ (T)assetList[file.srcFile].RefTarget() });
                return;
            }

            var ab = GetAssetBundle(path);
            if (ab == null)
            {
                return;
            }

            CoroutineMgr.StartCoroutine(DoGetAllResourcesAsync<T>(ab, onComplete));
        }

        #endregion


        #region getResource by number

        public UnityEngine.Object GetResource(int number)
        {
            var file = resourceTable.GetResourceFile(number);
            return file != null ? GetResource(file.srcFile) : null;
        }

        public void GetResourceAsync(int number, BKAction<UnityEngine.Object> onComplete)
        {
            var file = resourceTable.GetResourceFile(number);
            if (file != null)
            {
                GetResourceAsync(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public T GetResource<T>(int number) where T : UnityEngine.Object
        {
            var file = resourceTable.GetResourceFile(number);
            return file != null ? GetResource<T>(file.srcFile) : null;
        }

        public void GetResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            var file = resourceTable.GetResourceFile(number);
            if (file != null)
            {
                GetResourceAsync<T>(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public UnityEngine.Object[] GetAllResources(int number)
        {
            var file = resourceTable.GetResourceFile(number);
            return file != null ? GetAllResources(file.srcFile) : null;
        }

        public void GetAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete)
        {
            var file = resourceTable.GetResourceFile(number);
            if (file != null)
            {
                GetAllResourcesAsync(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public T[] GetAllResources<T>(int number) where T : UnityEngine.Object
        {
            var file = resourceTable.GetResourceFile(number);
            return file != null ? GetAllResources<T>(file.srcFile) : null;
        }

        public void GetAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            var file = resourceTable.GetResourceFile(number);
            if (file != null)
            {
                GetAllResourcesAsync<T>(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }


        #endregion


        #region add release remove

        /// <summary>
        /// add external loaded resource
        /// </summary>
        public void AddResource(Loader loader, bool cache = false)
        {
            if (!loader.hasError)
            {
                AddResource(loader.url, loader.GetOrgAssetBundle(), cache);
            }
        }

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        public void ReleaseResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                var item = assetList[path];
                item.DeRefTarget();
                if (item.CanDisposed())
                {
                    Resources.UnloadAsset(item.target as UnityEngine.Object);
                    assetList.Remove(path);
                }
                return;
            }

            string key = null;

            var file = resourceTable.GetResourceFile(path);
            if (file != null && assetBundleList.ContainsKey(file.targetFile))
            {
                key = file.targetFile;
            } else if (assetBundleList.ContainsKey(path))
            {
                key = path;
            }

            if (key != null)
            {
                var item = assetBundleList[key];
                item.DeRefTarget();
                if (item.CanDisposed())
                {
                    var ab = item.target as AssetBundle;
                    if (ab != null)
                    {
                        ab.Unload(false);
                    }
                    assetBundleList.Remove(key);
                }
            }
        }

        /// <summary>
        /// remove resource except in using, be carebuf when using
        /// </summary>
        public void RemoveResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                var item = assetList[path];
                item.DeRefTarget();
                if (item.CanDisposed())
                {
                    Resources.UnloadAsset(item.target as UnityEngine.Object);
                }
                assetList.Remove(path);
                return;
            }

            string key = null;

            var file = resourceTable.GetResourceFile(path);
            if (file != null && assetBundleList.ContainsKey(file.targetFile))
            {
                key = file.targetFile;
            } else if (assetBundleList.ContainsKey(path))
            {
                key = path;
            }

            if (key != null)
            {
                var item = assetBundleList[key];
                var ab = item.target as AssetBundle;
                if (ab != null)
                {
                    ab.Unload(false);
                }
                assetBundleList.Remove(key);
            }
        }

        /// <summary>
        /// remove all resources except in using, be carebuf when using
        /// </summary>
        public void RemoveAllResources()
        {
            foreach (var item in assetList.Values)
            {
                item.target = null;
            }
            assetList.Clear();
            Resources.UnloadUnusedAssets();

            foreach (var item in assetBundleList.Values)
            {
                var ab = item.target as AssetBundle;
                if (ab != null)
                {
                    ab.Unload(false);
                }
            }

            assetBundleList.Clear();
        }

        #endregion


        private void GetResourceFiles(IResourceFile resFile, List<IResourceFile> results)
        {
            if (resFile == null ||
                assetList.ContainsKey(resFile.srcFile) ||
                assetBundleList.ContainsKey(resFile.targetFile))
            {
                return;
            }

            results.Add(resFile);

            var dependencies = resFile.dependencies;
            if (dependencies != null)
            {
                foreach (var file in dependencies)
                {
                    GetResourceFiles(resourceTable.GetResourceFile(file), results);
                }
            }
        }

        private bool HasResource(IResourceFile resFile)
        {
            return assetList.ContainsKey(resFile.srcFile) ||
            (assetBundleList.ContainsKey(resFile.targetFile) &&
            assetBundleList[resFile.targetFile].target != null);
        }

        private void UpdateCache(string path, bool cache)
        {
            if (assetList.ContainsKey(path) && !assetList[path].cache)
            {
                assetList[path].cache = cache;
            } else
            {
                var file = resourceTable.GetResourceFile(path);
                if (file != null && assetBundleList.ContainsKey(file.targetFile) &&
                    !assetBundleList[file.targetFile].cache)
                {
                    assetList[path].cache = cache;
                }
            }
        }

        IEnumerator DoGetResourceAsync(IResourceFile file, AssetBundle ab, BKAction<UnityEngine.Object> onComplete)
        {
            var req = ab.LoadAssetAsync(file.idInPack, GetResourceType(file.type));
            yield return req;

            var obj = req.asset;
            BundleToAsset(file.srcFile, file, ab, obj);
            onComplete(obj);
        }

        IEnumerator DoGetResourceAsync<T>(IResourceFile file, AssetBundle ab, BKAction<T> onComplete) 
            where T : UnityEngine.Object
        {
            var req = ab.LoadAssetAsync<T>(file.idInPack);
            yield return req;

            var obj = (T)req.asset;
            BundleToAsset(file.srcFile, file, ab, obj);
            onComplete(obj);
        }

        IEnumerator DoGetAllResourcesAsync<T>(AssetBundle ab, BKAction<T[]> onComplete) 
            where T : UnityEngine.Object
        {
            var req = ab.LoadAllAssetsAsync<T>();
            yield return req;

            onComplete((T[])req.allAssets);
        }

        private Type GetResourceType(string type)
        {
            Type obj;

            //为什么要判断类型？因为unity打包可能重名
            switch (type)
            {
                case "GameObject":
                    obj = typeof(GameObject);
                    break;
                case "Texture2D":
                    obj = typeof(Texture2D);
                    break;
                case "Sprite":
                    obj = typeof(Sprite);
                    break;
                case "TextAsset":
                    obj = typeof(TextAsset);
                    break;
                case "AudioClip":
                    obj = typeof(AudioClip);
                    break;
                case "Animator":
                    obj = typeof(Animator);
                    break;
                case "Animation":
                    obj = typeof(Animation);
                    break;
                case "Material":
                    obj = typeof(Material);
                    break;
                case "Shader":
                    obj = typeof(Shader);
                    break;
                case "Mesh":
                    obj = typeof(Mesh);
                    break;
                default:
                    obj = typeof(UnityEngine.Object);
                    break;
            }

            return obj;
        }

        private string GetIdInPack(string path)
        {
            var idInPack = path;
            var pos = path.LastIndexOf('/') + 1;
            if (pos > 0)
            {
                idInPack = path.Substring(pos);
            }

            pos = idInPack.LastIndexOf('.');
            if (pos > 0)
            {
                idInPack = idInPack.Substring(0, pos);
            }

            return idInPack;
        }

        private AssetBundle GetAssetBundle(string path)
        {
            var file = resourceTable.GetResourceFile(path);
            if (file != null && assetBundleList.ContainsKey(file.targetFile))
            {
                return assetBundleList[file.targetFile].target as AssetBundle;
            }

            return assetBundleList.ContainsKey(path) ? assetBundleList[path].target as AssetBundle : null;
        }

        private void BundleToAsset(string path, IResourceFile file, AssetBundle ab, UnityEngine.Object asset)
        {
            if (file != null && file.singleDirectResource && !file.beDependent && asset != null &&
                (!assetBundleList.ContainsKey(file.targetFile) ||
                !assetBundleList[file.targetFile].cache))
            {
                var cref = new CountableRef(asset);
                cref.RefTarget();
                assetList.Add(file.srcFile, cref);
                ab.Unload(false);
                assetBundleList.Remove(file.targetFile);
            } else
            {
                if (assetBundleList.ContainsKey(path) && asset != null)
                {
                    assetBundleList[path].RefTarget();
                }
            }
        }

        private void AddResource(string url, AssetBundle ab, bool cache)
        {
            if (!assetBundleList.ContainsKey(url))
            {
                assetBundleList.Add(url, new CountableRef(ab, cache));
            } else
            {
                assetBundleList[url] = new CountableRef(ab, cache);
            }
        }
    }
}