using UnityEngine;
using System.Collections;
using UnityEditor;
using SevenZip.Compression.LZMA;
using System.IO;
using System;
using ICSharpCode.SharpZipLib.Zip;
using bookrpg.Editor;

public class Build : ScriptableObject
{

    private static string baseUrl = Application.dataPath + "/abs/";

    [MenuItem("Build/selected is sub asset?")]
    static void BuildSub()
    {
        PackProject pp = new PackProject();
        pp.pack.Add("test/*.prefab");

        var ap = new ResourcePacker();
        ap.namePattern = PackNamePattern.Hash;

        ap.Pack(pp, "../release/android/update", 
            BuildAssetBundleOptions.UncompressedAssetBundle,
            BuildTarget.StandaloneOSXUniversal
        );

        ap.GenerateResourceTable("../release/android/update/resourceTable.json");

    }

    [MenuItem("Build/Build LZMA Compressed Asset Bundles")]
    static void BuildCompressedABS()
    {
        var buildMap = new AssetBundleBuild[1];
        var build = new AssetBundleBuild();
        build.assetBundleName = "php-c";
        build.assetBundleVariant = "";
        build.assetNames = new string[]{ "Assets/php.bytes" };
        buildMap[0] = build;
//        BuildPipeline.BuildAssetBundles("Assets/abs", buildMap);

        BuildPipeline.BuildAssetBundles("Assets/abs", 
            BuildAssetBundleOptions.ForceRebuildAssetBundle);
        AssetDatabase.Refresh();
    }

    [MenuItem("Build/Build LZ4 Compressed Asset Bundles")]
    static void BuildCompressedABS2()
    {
        BuildPipeline.BuildAssetBundles("Assets/abs", 
            BuildAssetBundleOptions.ForceRebuildAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression);
        AssetDatabase.Refresh();
    }

    [MenuItem("Build/Build Uncompressed Asset Bundles")]
    static void BuildUncompressedABS()
    {
        BuildPipeline.BuildAssetBundles("Assets/abs", 
            BuildAssetBundleOptions.UncompressedAssetBundle |
            BuildAssetBundleOptions.ForceRebuildAssetBundle);
        AssetDatabase.Refresh();
    }


    [MenuItem("Build/CompressFile")]
    static void CompressFile()
    {
        CompressFileLZMA(baseUrl + "topbottom", baseUrl + "topbottom.7z");
        CompressZlib(baseUrl + "topbottom", baseUrl + "topbottom.zip");
        AssetDatabase.Refresh();

    }

    [MenuItem("Build/DecompressFile")]
    static void DecompressFile()
    {
        DecompressFileLZMA(baseUrl + "topbottom.7z", baseUrl + "topbottom.7z.d");
        DeCompressZlib(baseUrl + "topbottom.zip", baseUrl);
        AssetDatabase.Refresh();
    }

    private static void CompressZlib(string inFile, string outFile)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        using (ZipFile zip = ZipFile.Create(outFile))
        {
            zip.BeginUpdate();
            zip.Add(inFile, "topbottom.zip.d");
            zip.CommitUpdate();
        }

        stopwatch.Stop();
        Debug.LogFormat("zip encode time: {0}", stopwatch.Elapsed.TotalSeconds);
    }

    private static void DeCompressZlib(string inFile, string outFile)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        (new FastZip()).ExtractZip(inFile, outFile, "");

        stopwatch.Stop();
        Debug.LogFormat("zip decode time: {0}", stopwatch.Elapsed.TotalSeconds);

    }

    private static void CompressFileLZMA(string inFile, string outFile)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        // Write the encoder properties
        coder.WriteCoderProperties(output);

        // Write the decompressed file size.
        output.Write(BitConverter.GetBytes(input.Length), 0, 8);

        // Encode the file.
        coder.Code(input, output, input.Length, -1, null);
        output.Flush();
        output.Close();
        input.Flush();
        input.Close();

        stopwatch.Stop();
        Debug.LogFormat("7z encode time: {0}", stopwatch.Elapsed.TotalSeconds);
    }

    private static void DecompressFileLZMA(string inFile, string outFile)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
        input.Flush();
        input.Close();

        stopwatch.Stop();
        Debug.LogFormat("7z encode time: {0}", stopwatch.Elapsed.TotalSeconds);
    }

    [MenuItem("Build/Encode")]
    static void Encode()
    {
        string url = Application.dataPath + "/txt.bytes";
        var bytes = XXTEA.Encrypt(File.ReadAllBytes(url));
        File.WriteAllBytes(url.Replace("txt.bytes", "txt-encode.bytes"), bytes);
    }

    [MenuItem("Build/Decode")]
    static void Decode()
    {
        string url = Application.dataPath + "/txt-encode.bytes";
        var bytes = XXTEA.Decrypt(File.ReadAllBytes(url));
        File.WriteAllBytes(url.Replace("txt-encode.bytes", "txt-decode.txt"), bytes);
    }
}
