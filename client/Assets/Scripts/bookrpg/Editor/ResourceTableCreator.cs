using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using bookrpg.utils;
using bookrpg.resource;
using LitJson;

namespace bookrpg.Editor
{
    public class ResourceTableCreator
    {
        public string packOutputPath;
        public AssetBundleManifest manifest;
        public SortedList<string, ResourcePack> resourcePacks;

        private string historyPath;
        private SortedList<string, ResourcePack> lastResourcePacks;

        public ResourceTableCreator(
            string packOutputPath, 
            AssetBundleManifest manifest, 
            SortedList<string, ResourcePack> resourcePacks)
        {
            packOutputPath = packOutputPath.TrimEnd(new char[]{ '/', '\\' }) + "/";
            this.packOutputPath = packOutputPath;
            this.manifest = manifest;
            this.resourcePacks = resourcePacks;

            historyPath = Application.dataPath + "/../lastResourcePacks.json";
            loadHistory();
        }

        public void updateInfo()
        {
            if (manifest == null)
            {
                return;
            }

            var files = manifest.GetAllAssetBundles();

            var bundleDeps = new Dictionary<string, int>();

            foreach (var item in files)
            {
                foreach (var item2 in manifest.GetAllDependencies(item))
                {
                    if (bundleDeps.ContainsKey(item2))
                    {
                        bundleDeps[item2]++;
                    } else
                    {
                        bundleDeps[item2] = 1;
                    }
                }
            }

            foreach (var pack in resourcePacks.Values)
            {
                var packPath = packOutputPath + pack.srcFile;
                var info = new FileInfo(packPath);
                pack.size = (int)info.Length;
                pack.beDependent = bundleDeps.ContainsKey(pack.srcFile) &&
                bundleDeps[pack.srcFile] > 0;
                uint crc;
                if (BuildPipeline.GetCRCForAssetBundle(packPath, out crc))
                {
                    pack.crc = crc;
                }
                Hash128 hash;
                if (BuildPipeline.GetHashForAssetBundle(packPath, out hash))
                {
                    pack.hash = hash.ToString();
                }
                pack.dependencies = manifest.GetAllDependencies(pack.srcFile);

                ResourcePack lastpack;
                if (lastResourcePacks.TryGetValue(pack.srcFile, out lastpack))
                {
                    pack.version = pack.hash != lastpack.hash ? lastpack.version + 1 : lastpack.version;
                } else
                {
                    pack.version = 1;
                }
            }

            lastResourcePacks = resourcePacks;
            saveHistory();
        }

        public void generateResourceTable(string outputFile)
        {
            var resourcePacksJson = new List<string>();
            resourcePacksJson.Add("[");
            foreach (var item in resourcePacks.Values)
            {
                resourcePacksJson.Add(item.toReleaseJson() + ",");
            }
            resourcePacksJson.Add("]");

            File.WriteAllText(outputFile, string.Join("\n", resourcePacksJson.ToArray()));
        }

        private void loadHistory()
        {
            lastResourcePacks = new SortedList<string, ResourcePack>();

            if (File.Exists(historyPath))
            {
                var obj = JsonMapper.ToObject(File.ReadAllText(historyPath));
                if (obj != null)
                {
                    for (int i = 0; i < obj.Count; i++)
                    {
                        var data = obj[i];
                        var file = new ResourcePack();
                        file.fromJson(data);
                        lastResourcePacks.Add(file.srcFile, file);
                    }
                }
            }
        }

        private void saveHistory()
        {
            var jw = new JsonWriter();
            jw.PrettyPrint = true;
            JsonMapper.ToJson(new List<ResourcePack>(lastResourcePacks.Values), jw);
            File.WriteAllText(historyPath, jw.ToString());
        }
    }
}
