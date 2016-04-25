using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bookrpg.log;
using bookrpg.utils;

namespace bookrpg.resource
{

    public class ResourceBundle
    {
        protected AssetBundle assetBundle;

        public ResourceBundle()
        {
        }

        public object mainAsset
        { 
            get { return assetBundle.mainAsset; } 
        }

        public bool contains(string name)
        {
            return assetBundle.Contains(name);
        }

        public string[] getAllAssetNames()
        {
            return assetBundle.GetAllAssetNames();
        }

        public string[] getAllScenePaths()
        {
            return assetBundle.GetAllAssetNames();
        }

        ///////////////////////////////////////////

        public object[] lLoadAllAssets()
        {
            return assetBundle.LoadAllAssets();
        }

        public object[] loadAllAssets(Type type)
        {
            return assetBundle.LoadAllAssets(type);
        }

        public T[] loadAllAssets<T>() where T : UnityEngine.Object
        {
            return assetBundle.LoadAllAssets<T>();
        }

        ///////////////////////////////////////////

        public AssetBundleRequest loadAllAssetsAsync()
        {
            return assetBundle.LoadAllAssetsAsync();
        }

        public AssetBundleRequest loadAllAssetsAsync(Type type)
        {
            return assetBundle.LoadAllAssetsAsync(type);
        }

        public AssetBundleRequest loadAllAssetsAsync<T>()
        {
            return assetBundle.LoadAllAssetsAsync<T>();
        }

        ///////////////////////////////////////////

        public object loadAsset(string name)
        {
            return assetBundle.LoadAsset(name);
        }

        public T loadAsset<T>(string name) where T : UnityEngine.Object
        {
            return assetBundle.LoadAsset<T>(name);
        }

        public object loadAsset(string name, Type type)
        {
            return assetBundle.LoadAsset(name, type);
        }

        ///////////////////////////////////////////

        public AssetBundleRequest loadAssetAsync(string name)
        {
            return assetBundle.LoadAssetAsync(name);
        }

        public AssetBundleRequest loadAssetAsync<T>(string name)
        {
            return assetBundle.LoadAssetAsync<T>(name);
        }

        public AssetBundleRequest loadAssetAsync(string name, Type type)
        {
            return assetBundle.LoadAssetAsync(name, type);
        }

        ///////////////////////////////////////////

        public object[] loadAssetWithSubAssets(string name)
        {
            return assetBundle.LoadAssetWithSubAssets(name);
        }

        public T[] loadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            return assetBundle.LoadAssetWithSubAssets<T>(name);
        }

        public object[] loadAssetWithSubAssets(string name, Type type)
        {
            return assetBundle.LoadAssetWithSubAssets(name, type);
        }

        ///////////////////////////////////////////

        public AssetBundleRequest loadAssetWithSubAssetsAsync(string name)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync(name);
        }

        public AssetBundleRequest loadAssetWithSubAssetsAsync<T>(string name)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync<T>(name);
        }

        public AssetBundleRequest loadAssetWithSubAssetsAsync(string name, Type type)
        {
            return assetBundle.LoadAssetWithSubAssetsAsync(name, type);
        }

        public void unload(bool unloadAllLoadedObjects)
        {
            
        }

    }
}