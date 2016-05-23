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
            resourceTable = new EmptyResourceTable();
        }

        public ResourceMgrImpl(IResourceTable table)
        {
            resourceTable = table;
        }

        public Loader load(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            var bReturn = false;
            if (string.IsNullOrEmpty(path))
            {
                bReturn = true;
                Debug.LogWarning("LocalResMgr.load, path is empty");
            }
            if (hasResource(path))
            {
                bReturn = true;
                updateCache(path, cache);
            }
            if (bReturn)
            {
                if (onComplete != null)
                {
                    onComplete(path);
                }
                return null;
            }

            var file = resourceTable.getResourceFile(path);
            var loader = file == null ? LoaderMgr.load(path) : 
                LoaderMgr.load(file.targetFile, file.version, file.size);

            loader.onComplete += ld =>
            {
                if (!ld.hasError)
                {
                    addResource(path, ld.getOrgAssetBundle(), cache);
                }

                if (onComplete != null)
                {
                    onComplete(path);
                }

                ld.Dispose();
            };

            return loader;
        }

        public BatchLoader loadWithDependencies(string path, BKAction<string> onComplete = null, bool cache = false)
        {
            if (hasResource(path))
            {
                return null;
            }

            var batch = LoaderMgr.loadBatch();
            var file = resourceTable.getResourceFile(path);
            if (file == null)
            {
                batch.addLoader(path);
            } else
            {
                if (file.dependencies != null)
                {
                    foreach (var filestr in file.dependencies)
                    {
                        var file2 = resourceTable.getResourceFile(filestr);
                        if (!hasResource(file2))
                        {
                            batch.addLoader(file2.targetFile, file2.version, file2.size);
                        }
                    }
                }

                batch.addLoader(file.targetFile, file.version, file.size);
            }

            if (batch.getLoaders().Count == 0)
            {
                batch.disposeImmediate();
                if (onComplete != null)
                {
                    onComplete(path);
                }
                return null;
            }

            batch.onComplete += bt =>
            {
                foreach (var item in bt.getLoaders())
                {
                    if (!item.Value.hasError)
                    {
                        addResource(item.Key, item.Value.getOrgAssetBundle(), cache);
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

        public BatchLoader loadBatch(ICollection<string> pathes, BKAction<ICollection<string>> onComplete = null, bool cache = false)
        {
            var batch = LoaderMgr.loadBatch();
            
            foreach (var path in pathes)
            {
                if (hasResource(path))
                {
                    continue;
                }

                var file = resourceTable.getResourceFile(path);
                if (file == null)
                {
                    batch.addLoader(path);
                } else
                {
                    if (file.dependencies != null)
                    {
                        foreach (var filestr in file.dependencies)
                        {
                            var file2 = resourceTable.getResourceFile(filestr);
                            if (!hasResource(file2))
                            {
                                batch.addLoader(file2.targetFile, file2.version, file2.size);
                            }
                        }
                    }

                    batch.addLoader(file.targetFile, file.version, file.size);
                }
            }

            if (batch.getLoaders().Count == 0)
            {
                batch.disposeImmediate();
                if (onComplete != null)
                {
                    onComplete(pathes);
                }
                return null;
            }

            batch.onComplete += bt =>
            {
                foreach (var item in bt.getLoaders())
                {
                    if (!item.Value.hasError)
                    {
                        addResource(item.Key, item.Value.getOrgAssetBundle(), cache);
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

        public bool hasResource(string path)
        {
            //后者表示资源就是AssetBundle包本身，而不是包中的文件
            if (assetList.ContainsKey(path) ||
                (assetBundleList.ContainsKey(path) &&
                assetBundleList[path].target != null))
            {
                return true;
            }

            var file = resourceTable.getResourceFile(path);
            return file != null && hasResource(file);
        }

        public bool hasResource(int number)
        {
            var file = resourceTable.getResourceFile(number);
            return file != null && hasResource(file);
        }

        #region getResource by path

        public UnityEngine.Object getResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                return assetList[path].refTarget() as UnityEngine.Object;
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                return null;
            }

            IResourceFile file = resourceTable.getResourceFile(path);
            var obj = file == null ? ab : ab.LoadAsset(file.idInPack, getResourceType(file.type));
            bundleToAsset(path, file, ab, obj);
            return obj;
        }

        public void getResourceAsync(string path, BKAction<UnityEngine.Object> onComplete)
        {
            if (assetList.ContainsKey(path))
            {
                onComplete((assetList[path].refTarget() as UnityEngine.Object));
                return;
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                onComplete(null);
                return;
            }

            IResourceFile file = resourceTable.getResourceFile(path);
            if (file == null)
            {
                bundleToAsset(path, file, ab, ab);
                onComplete(ab);
                return;
            }

            CoroutineMgr.startCoroutine(doGetResourceAsync(file, ab, onComplete));
        }

        public T getResource<T>(string path) where T : UnityEngine.Object
        {
            if (assetList.ContainsKey(path))
            {
                return (T)(assetList[path].refTarget());
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                return default(T);
            }

            IResourceFile file = resourceTable.getResourceFile(path);
            if (file == null)
            {
                bundleToAsset(path, file, ab, ab);
                return (T)Convert.ChangeType(ab, typeof(T));
            } else
            {
                var obj = ab.LoadAsset<T>(file.idInPack);
                bundleToAsset(path, file, ab, obj);
                return obj;
            }
        }

        public void getResourceAsync<T>(string path, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            if (assetList.ContainsKey(path))
            {
                onComplete((T)(assetList[path].refTarget()));
                return;
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                onComplete(default(T));
                return;
            }

            IResourceFile file = resourceTable.getResourceFile(path);
            if (file == null)
            {
                bundleToAsset(path, file, ab, ab);
                onComplete((T)Convert.ChangeType(ab, typeof(T)));
                return;
            }

            CoroutineMgr.startCoroutine(doGetResourceAsync<T>(file, ab, onComplete));
        }

        #endregion

        #region getAllResources

        public UnityEngine.Object[] getAllResources(string path)
        {
            IResourceFile file = resourceTable.getResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                return new UnityEngine.Object[]{ assetList[file.srcFile].refTarget() as UnityEngine.Object };
            }

            var ab = getAssetBundle(path);
            return ab == null ? null : ab.LoadAllAssets();
        }

        public void getAllResourcesAsync(string path, BKAction<UnityEngine.Object[]> onComplete)
        {
            IResourceFile file = resourceTable.getResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                onComplete(new UnityEngine.Object[]{ assetList[file.srcFile].refTarget() as UnityEngine.Object });
                return;
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                return;
            }

            CoroutineMgr.startCoroutine(doGetAllResourcesAsync<UnityEngine.Object>(ab, onComplete));
        }

        public T[] getAllResources<T>(string path) where T : UnityEngine.Object
        {
            IResourceFile file = resourceTable.getResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                return new T[]{ (T)assetList[file.srcFile].refTarget() };
            }

            var ab = getAssetBundle(path);
            return ab == null ? null : (T[])ab.LoadAllAssets();
        }

        public void getAllResourcesAsync<T>(string path, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {

            IResourceFile file = resourceTable.getResourceFile(path);
            if (file != null && file.singleDirectResource && assetList.ContainsKey(file.srcFile))
            {
                onComplete(new T[]{ (T)assetList[file.srcFile].refTarget() });
                return;
            }

            var ab = getAssetBundle(path);
            if (ab == null)
            {
                return;
            }

            CoroutineMgr.startCoroutine(doGetAllResourcesAsync<T>(ab, onComplete));
        }

        #endregion


        #region getResource by number

        public UnityEngine.Object getResource(int number)
        {
            var file = resourceTable.getResourceFile(number);
            return file != null ? getResource(file.srcFile) : null;
        }

        public void getResourceAsync(int number, BKAction<UnityEngine.Object> onComplete)
        {
            var file = resourceTable.getResourceFile(number);
            if (file != null)
            {
                getResourceAsync(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public T getResource<T>(int number) where T : UnityEngine.Object
        {
            var file = resourceTable.getResourceFile(number);
            return file != null ? getResource<T>(file.srcFile) : null;
        }

        public void getResourceAsync<T>(int number, BKAction<T> onComplete) where T : UnityEngine.Object
        {
            var file = resourceTable.getResourceFile(number);
            if (file != null)
            {
                getResourceAsync<T>(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public UnityEngine.Object[] getAllResources(int number)
        {
            var file = resourceTable.getResourceFile(number);
            return file != null ? getAllResources(file.srcFile) : null;
        }

        public void getAllResourcesAsync(int number, BKAction<UnityEngine.Object[]> onComplete)
        {
            var file = resourceTable.getResourceFile(number);
            if (file != null)
            {
                getAllResourcesAsync(file.srcFile, onComplete);
            } else
            {
                onComplete(null);
            }
        }

        public T[] getAllResources<T>(int number) where T : UnityEngine.Object
        {
            var file = resourceTable.getResourceFile(number);
            return file != null ? getAllResources<T>(file.srcFile) : null;
        }

        public void getAllResourcesAsync<T>(int number, BKAction<T[]> onComplete) where T : UnityEngine.Object
        {
            var file = resourceTable.getResourceFile(number);
            if (file != null)
            {
                getAllResourcesAsync<T>(file.srcFile, onComplete);
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
        public void addResource(Loader loader, bool cache = false)
        {
            if (!loader.hasError)
            {
                addResource(loader.url, loader.getOrgAssetBundle(), cache);
            }
        }

        /// <summary>
        /// when resource's refcount is 0, then dispose it.
        /// </summary>
        public void releaseResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                var item = assetList[path];
                item.deRefTarget();
                if (item.canDisposed())
                {
                    Resources.UnloadAsset(item.target as UnityEngine.Object);
                    assetList.Remove(path);
                }
                return;
            }

            string key = null;

            var file = resourceTable.getResourceFile(path);
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
                item.deRefTarget();
                if (item.canDisposed())
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
        public void removeResource(string path)
        {
            if (assetList.ContainsKey(path))
            {
                var item = assetList[path];
                item.deRefTarget();
                if (item.canDisposed())
                {
                    Resources.UnloadAsset(item.target as UnityEngine.Object);
                }
                assetList.Remove(path);
                return;
            }

            string key = null;

            var file = resourceTable.getResourceFile(path);
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
        public void removeAllResources()
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


        private void getResourceFiles(IResourceFile resFile, List<IResourceFile> results)
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
                    getResourceFiles(resourceTable.getResourceFile(file), results);
                }
            }
        }

        private bool hasResource(IResourceFile resFile)
        {
            return assetList.ContainsKey(resFile.srcFile) ||
            (assetBundleList.ContainsKey(resFile.targetFile) &&
            assetBundleList[resFile.targetFile].target != null);
        }

        private void updateCache(string path, bool cache)
        {
            if (assetList.ContainsKey(path) && !assetList[path].cache)
            {
                assetList[path].cache = cache;
            } else
            {
                var file = resourceTable.getResourceFile(path);
                if (file != null && assetBundleList.ContainsKey(file.targetFile) &&
                    !assetBundleList[file.targetFile].cache)
                {
                    assetList[path].cache = cache;
                }
            }
        }

        IEnumerator doGetResourceAsync(IResourceFile file, AssetBundle ab, BKAction<UnityEngine.Object> onComplete)
        {
            var req = ab.LoadAssetAsync(file.idInPack, getResourceType(file.type));
            yield return req;

            var obj = req.asset;
            bundleToAsset(file.srcFile, file, ab, obj);
            onComplete(obj);
        }

        IEnumerator doGetResourceAsync<T>(IResourceFile file, AssetBundle ab, BKAction<T> onComplete) 
            where T : UnityEngine.Object
        {
            var req = ab.LoadAssetAsync<T>(file.idInPack);
            yield return req;

            var obj = (T)req.asset;
            bundleToAsset(file.srcFile, file, ab, obj);
            onComplete(obj);
        }

        IEnumerator doGetAllResourcesAsync<T>(AssetBundle ab, BKAction<T[]> onComplete) 
            where T : UnityEngine.Object
        {
            var req = ab.LoadAllAssetsAsync<T>();
            yield return req;

            onComplete((T[])req.allAssets);
        }

        private Type getResourceType(string type)
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

        private string getIdInPack(string path)
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

        private AssetBundle getAssetBundle(string path)
        {
            var file = resourceTable.getResourceFile(path);
            if (file != null && assetBundleList.ContainsKey(file.targetFile))
            {
                return assetBundleList[file.targetFile].target as AssetBundle;
            }

            return assetBundleList.ContainsKey(path) ? assetBundleList[path].target as AssetBundle : null;
        }

        private void bundleToAsset(string path, IResourceFile file, AssetBundle ab, UnityEngine.Object asset)
        {
            if (file != null && file.singleDirectResource && !file.beDependent && asset != null &&
                (!assetBundleList.ContainsKey(file.targetFile) ||
                !assetBundleList[file.targetFile].cache))
            {
                var cref = new CountableRef(asset);
                cref.refTarget();
                assetList.Add(file.srcFile, cref);
                ab.Unload(false);
                assetBundleList.Remove(file.targetFile);
            } else
            {
                if (assetBundleList.ContainsKey(path) && asset != null)
                {
                    assetBundleList[path].refTarget();
                }
            }
        }

        private void addResource(string url, AssetBundle ab, bool cache)
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

    internal class EmptyResourceTable : IResourceTable
    {
        public IResourceFile getResourceFile(string resourcePath)
        {
            return null;
        }

        public IResourceFile getResourceFile(int resourceNumber)
        {
            return null;
        }
    }
}