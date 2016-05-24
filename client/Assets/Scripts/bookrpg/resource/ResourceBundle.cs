using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.log;
using bookrpg.core;
using bookrpg.utils;

namespace bookrpg.resource
{

    public class ResourceBundle : IDispose
    {
        protected AssetBundle assetBundle;

        public ResourceBundle(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }

        public void Dispose()
        {
            assetBundle = null;
            hasDisposed = true;
        }

        public bool hasDisposed
        {
            get;
            private set;
        }

        public object mainAsset
        { 
            get { return assetBundle.mainAsset; } 
        }

        public bool Contains(string name)
        {
            return assetBundle.Contains(name);
        }

        public string[] GetAllAssetNames()
        {
            return assetBundle.GetAllAssetNames();
        }

        public string[] GetAllScenePaths()
        {
            return assetBundle.GetAllAssetNames();
        }

        ///////////////////////////////////////////

        public object[] LLoadAllAssets()
        {
            return assetBundle.LoadAllAssets();
        }

        public object[] LoadAllAssets(Type type)
        {
            return assetBundle.LoadAllAssets(type);
        }

        public T[] LoadAllAssets<T>() where T : UnityEngine.Object
        {
            return assetBundle.LoadAllAssets<T>();
        }

        ///////////////////////////////////////////

        public AssetBundleRequest LoadAllAssetsAsync()
        {
            return assetBundle.LoadAllAssetsAsync();
        }

        public AssetBundleRequest LoadAllAssetsAsync(Type type)
        {
            return assetBundle.LoadAllAssetsAsync(type);
        }

        public AssetBundleRequest LoadAllAssetsAsync<T>()
        {
            return assetBundle.LoadAllAssetsAsync<T>();
        }

        ///////////////////////////////////////////

        public object LoadAsset(string name)
        {
            return assetBundle.LoadAsset(name);
        }

        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            return assetBundle.LoadAsset<T>(name);
        }

        public object LoadAsset(string name, Type type)
        {
            return assetBundle.LoadAsset(name, type);
        }

        ///////////////////////////////////////////

        public AssetBundleRequest LoadAssetAsync(string name)
        {
            return assetBundle.LoadAssetAsync(name);
        }

        public AssetBundleRequest LoadAssetAsync<T>(string name)
        {
            return assetBundle.LoadAssetAsync<T>(name);
        }

        public AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            return assetBundle.LoadAssetAsync(name, type);
        }

        ///////////////////////////////////////////

        public object[] LoadAssetWithSubAssets(string name)
        {
            return assetBundle.LoadAssetWithSubAssets(name);
        }

        public T[] LoadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            return assetBundle.LoadAssetWithSubAssets<T>(name);
        }

        public object[] LoadAssetWithSubAssets(string name, Type type)
        {
            return assetBundle.LoadAssetWithSubAssets(name, type);
        }

        ///////////////////////////////////////////

        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync(name);
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync<T>(name);
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync(name, type);
        }

        public void Unload(bool unloadAllLoadedObjects)
        {
            
        }

    }
}