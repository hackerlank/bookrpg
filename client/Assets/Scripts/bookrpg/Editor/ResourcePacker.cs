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

    public enum PackNamePattern
    {
        FileName,
        PathName,
        Hash,
        FileNameWithHash,
        FileNameWithVersion,
        PathNameWithHash,
        PathNameWithVersion,
    }

    public class ResourcePacker
    {
        private string comment = "#";
        private string appDir;
        private PackProject project;

        //packName->ResourcePack
        private SortedList<string, ResourcePack> resourcePacks;
        //resourcePath->ResourcePack
        private Dictionary<string, ResourcePack> resources;

        private List<string> editorPackedAssets = new List<string>();

        private string tmpPackPath;
        private AssetBundleManifest manifest;
        private string outputPath;

        public ICodec encryption;
        public ICodec compression;

        public PackNamePattern namePattern = PackNamePattern.PathName;

        public bool deleteManifest = true;

        public string mainManifestName = "manifest";

        private string AssetsDir = "Assets";

        private ResourceTableCreator resourceTableCreator;

        public ResourcePacker()
        {
            appDir = Regex.Replace(Application.dataPath, AssetsDir + "$", "", RegexOptions.IgnoreCase);
            tmpPackPath = appDir + "ResourcePack/";
        }

        public void Pack(
            string projectFile,
            string outputPath, 
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None, 
            BuildTarget target = BuildTarget.WebPlayer)
        {
            if (!File.Exists(projectFile))
            {
                Debug.LogError("projectFile not exist :" + projectFile);
                return;
            }

            var str = File.ReadAllText(projectFile);
            Pack(PackProject.FromJson(str), outputPath);
        }

        public void Pack(
            PackProject project, 
            string outputPath, 
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None, 
            BuildTarget target = BuildTarget.WebPlayer)
        {
            this.project = project;
            this.outputPath = Util.GetAbsolutePath(outputPath, appDir);
            this.tmpPackPath += target.ToString() + "/" + mainManifestName + "/";
            
            CreatePack();

            DoPack(options, target);
        }

        public void GenerateResourceTable(string outputFile)
        {
            resourceTableCreator.GenerateResourceTable(outputFile);
        }

        private void CreatePack()
        {
            var files = new List<string>();
            editorPackedAssets.Clear();
            resourcePacks = new SortedList<string, ResourcePack>();
            resources = new Dictionary<string, ResourcePack>();

            foreach (var item in AssetDatabase.GetAllAssetBundleNames())
            {
                var tmp = AssetDatabase.GetAssetPathsFromAssetBundle(item);
                if (tmp.Length > 0)
                {
                    files.AddRange(tmp);
                    editorPackedAssets.AddRange(tmp);
                    CreateResourcePack(item, tmp);
                }
            }

            foreach (var item in project.pack)
            {
                var tmp = FilterFiles(ScanFiles(item));
                files.AddRange(tmp);
                CreateResourcePack(tmp);
            }

            foreach (var item in project.bulk)
            {
                var tmp = new List<string>(); 
                foreach (var item2 in item.Value)
                {
                    tmp.AddRange(FilterFiles(ScanFiles(item2)));
                }

                files.AddRange(tmp);
                CreateResourcePack(item.Key, tmp.ToArray());
            }

            var depsCount = new Dictionary<string, int>();
            var deps = AssetDatabase.GetDependencies(files.ToArray(), true);

            foreach (var item in deps)
            {
                foreach (var item2 in AssetDatabase.GetDependencies(item, false))
                {
                    if (depsCount.ContainsKey(item2))
                    {
                        depsCount[item2]++;
                    } else
                    {
                        depsCount.Add(item2, 1);
                    }
                }
            }

            foreach (var item in depsCount)
            {
                //非直接使用的资源，多次间接使用，拆分打包
                if (item.Value > 1 && !files.Contains(item.Key))
                {
                    CreateResourcePack(item.Key);
                }
            }
        }

        private void DoPack(BuildAssetBundleOptions options, BuildTarget target)
        {
            if (!Directory.Exists(tmpPackPath))
            {
                Directory.CreateDirectory(tmpPackPath);
            }

            var packList = resourcePacks.Values;
            var builds = new AssetBundleBuild[packList.Count];
            for (int i = 0; i < packList.Count; i++)
            {
                builds[i] = packList[i].CreateBuild();
            }

            options &= ~BuildAssetBundleOptions.AppendHashToAssetBundleName;
            manifest = BuildPipeline.BuildAssetBundles(tmpPackPath, builds, options, target);
            //去掉上面一部产生的AssetBundleName
            AssetDatabase.RemoveUnusedAssetBundleNames();

            CreateResourcePack(mainManifestName);

            resourceTableCreator = new ResourceTableCreator(tmpPackPath, manifest, resourcePacks);
            resourceTableCreator.UpdateInfo();

            CopyPacks(!deleteManifest);
        }

        private void CopyPacks(bool copyManifest = true)
        {
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            Directory.CreateDirectory(outputPath);

            foreach (var item in resourcePacks.Values)
            {
                item.CreateTargetFile(namePattern);
                File.Copy(tmpPackPath + item.srcFile, outputPath + item.targetFile);
                if (copyManifest)
                {
                    File.Copy(tmpPackPath + item.srcFile + ".manifest", 
                        outputPath + item.targetFile + ".manifest");
                }
            }
        }

        private string[] ScanFiles(string path)
        {
            path = appDir + AssetsDir + "/" + path;
            return Util.ScanFiles(path);
        }

        //one in one pack
        private void CreateResourcePack(string path)
        {
            CreateResourcePack(CreateResourcePackName(path), new string[]{ path });
        }

        //all in each pack
        private void CreateResourcePack(string[] pathes)
        {
            foreach (var item in pathes)
            {
                CreateResourcePack(CreateResourcePackName(item), new string[]{ item });
            }
        }

        //all in one pack
        private void CreateResourcePack(string bundleName, string[] pathes)
        {
            var pack = new ResourcePack();
            pack.srcFile = bundleName;
            pack.compression = compression != null ? compression.name : "";
            pack.encryption = encryption != null ? encryption.name : "";
            pack.resources = pathes;

            foreach (var item in pathes)
            {
                if (resources.ContainsKey(item))
                {
                    Debug.LogWarningFormat("Repeated pack {0} in {1} and {2}", 
                        item, resources[item].srcFile, pack.srcFile);
                }
            }

            resourcePacks.Add(pack.srcFile, pack);
        }

        private string CreateResourcePackName(string path)
        {
            return Regex.Replace(path, "^" + AssetsDir + "[\\|/]", "", RegexOptions.IgnoreCase).
                Replace('/', '-').Replace('.', '_');
        }

        private string[] FilterFiles(string[] files)
        {
            var list = new List<string>();
            foreach (var file in files)
            {
                if (!IgnoreFile(file))
                {
                    list.Add(file.Replace(appDir, ""));
                }
            }

            return list.ToArray();
        }

        private bool IgnoreFile(string path)
        {
            string name = path.Substring(path.LastIndexOf('/') + 1);
            if (name.StartsWith(comment))
            {
                return true;
            }

            var dir = appDir + AssetsDir + "/";

            foreach (var pattern in project.exclude)
            {
                if (Util.WildcardMatch(path.Replace(dir, ""), pattern))
                {
                    return true;
                }
            }

            path = path.Replace(appDir, "");
            if (editorPackedAssets.Contains(path))
            {
                return true;
            }

            return false;
        }

        public void Test()
        {
            var target = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var deps = AssetDatabase.GetDependencies(target, true);
            Debug.Log(string.Join("\n", deps));

            byte[] a = File.ReadAllBytes(target);
            var str = CRC32.GetHash(a);

            uint crc;

            BuildPipeline.GetCRCForAssetBundle(target, out crc);
            Debug.Log(crc);
            Debug.Log(str);
        }

    }
}