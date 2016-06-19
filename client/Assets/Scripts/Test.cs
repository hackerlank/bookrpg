using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using bookrpg.log;
using bookrpg.mgr;
using bookrpg.core;
using bookrpg.resource;
using bookrpg.utils;
using bookrpg.net;
using ICSharpCode.SharpZipLib.Zip;

public class Test : MonoBehaviour
{

    string baseUrl = "/Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";
    string baseUrlFile = "file:///Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";

    WWW www;

    AssetBundle ab;
    AssetBundle ab2;

    byte[] bytes;

    UnityEngine.Object[] textureArr;

    ResourceMgrImpl mgr;


    public void  LoadAssetBundle()
    {

        string uncompress = baseUrlFile + "ResourcePack_00000000000000000000000000000000";
        string compress = baseUrlFile + "scenec";

//        StartCoroutine(Load(uncompress));


//        StartCoroutine(DoLoadAssetBundle(uncompress));

//        LoadTest();


        var b = new ByteArray();
        b.endian = Endian.BIG_ENDIAN;
        b.Write((short)-1);
        b.Write((int)-1);
        b.Write((long)-1);
        b.Write((float)-1.1);
        b.Write(-1.1);

        b.position = 0;
        Debug.Log(b.ReadInt16());
        Debug.Log(b.ReadInt32());
        Debug.Log(b.ReadInt64());
        Debug.Log(b.ReadSingle());
        Debug.Log(b.ReadDouble());
       
        File.WriteAllBytes("/Users/llj/Downloads/1.txt", b.ToArray());

        return;


        txt.text = "little:\n";

        var f = "llj";
        Debug.Log(f);

        ByteArray bytes = new ByteArray();
        bytes.endian = Endian.LITTLE_ENDIAN;
        bytes.Write(f);
        bytes.position = 0;
        while (bytes.bytesAvailable > 0)
        {
            txt.text += bytes.ReadSByte() + " ";
        }
        bytes.position = 0;

        txt.text += "\n" + bytes.ReadString();

        txt.text += "\nbig:\n";

        bytes = new ByteArray();
        bytes.endian = Endian.BIG_ENDIAN;
        bytes.Write(f);
        bytes.position = 0;
        while (bytes.bytesAvailable > 0)
        {
            txt.text += bytes.ReadSByte() + " ";
        }
        bytes.position = 0;

        txt.text += "\n" + bytes.ReadString();

    }

    TcpClient tcp;

    private void LoadTest()
    {
        tcp = new TcpClient();
        tcp.onReceive += (tp, bytes) =>
        {
            Debug.Log("rec:" + bytes.Length.ToString());
        };
            
        tcp.Connect("127.0.0.1", 2000);

    }

    private IEnumerator DoLoadAssetBundle(string url)
    {
        var t1 = Time.time;
        www = new WWW(url);

        yield return www;

        Debug.LogFormat("loadAssetBundle: {0}, time: {1}", url, Time.time - t1);

        ab = www.assetBundle;

        var names = ab.GetAllAssetNames();

        var cc = ab.LoadAssetWithSubAssets("assets/test/cube.prefab");

        www.Dispose();
        www = null;
    }


    IEnumerator Load(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        ab2 = www.assetBundle;
        Debug.Log(www.url);

        var names = ab2.GetAllAssetNames();

        var main = ab2.LoadAsset<AssetBundleManifest>(names[0]);

        var abs = main.GetAllAssetBundles();

        var absv = main.GetAllAssetBundlesWithVariant();

        foreach (var item in abs)
        {
//            main.a
        }

        StartCoroutine(DoLoadAssetBundle(baseUrlFile + "cube.cn"));
    }

    public void LoadTexture()
    {
        if (ab == null)
        {
            var bytes = DecompressLZMA();
            ab = AssetBundle.LoadFromMemory(bytes);
        }

        textureArr = ab.LoadAllAssets();

//        var text = textureArr[0] as TextAsset;
//        byte[] bytes = XXTEA.Decrypt(text.bytes);
//
//        File.WriteAllBytes(Application.dataPath + "/txt-decode.txt", bytes);

//        ab.Unload(false);
//        ab = null;



        Debug.LogFormat("loadTexture: {0}", textureArr.Length.ToString());


//        if (ab != null)
//        {
//            ab.Unload(false);
//            ab = null;
//        }
//        if (ab2 != null)
//        {
//            ab2.Unload(false);
//            ab2 = null;
//        }

        GC.Collect();
    }

    public void DisposeTexture()
    {
        for (int i = 0; i < textureArr.Length; i++)
        {
            textureArr[i] = null;
        }

        Resources.UnloadUnusedAssets();

        GC.Collect();
    }


    public Text txt;

    private byte[] DecompressLZMA()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();

        var bytes = DecompressFileLZMA(baseUrl + "map.7z", baseUrl + "map.7z.d");

        stopwatch.Stop();

        if (txt != null)
        {
            txt.text += string.Format("7z decode time: {0} \n", stopwatch.Elapsed.TotalSeconds);
        }

       

        Debug.Log(stopwatch.Elapsed.TotalSeconds); //这里是输出的总运行秒数,精确到毫秒的

        return bytes;

    }

    private void DecompressZLIB()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();

        (new FastZip()).ExtractZip(baseUrl + "map.zip", baseUrl, "");

        stopwatch.Stop();

        if (txt != null)
        {
            txt.text += string.Format("zip decode time: {0}\n", stopwatch.Elapsed.TotalSeconds);
        }

        Debug.Log(stopwatch.Elapsed.TotalSeconds); //这里是输出的总运行秒数,精确到毫秒的
    }

    private IEnumerator DecompressUnity()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();

        WWW w = new WWW(baseUrl + "map.u");

        yield return w;

        stopwatch.Stop();

        if (txt != null)
        {
            txt.text += string.Format("unity decode time: {0}\n", stopwatch.Elapsed.TotalSeconds);
        }

        Debug.Log(stopwatch.Elapsed.TotalSeconds); //这里是输出的总运行秒数,精确到毫秒的
    }


    Loader loader;

    void Start()
    {

    }


    void Update()
    {

    }


    private static void CompressFileLZMA(string inFile, string outFile)
    {
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
    }

    private static byte[] DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
//        FileStream output = new FileStream(outFile, FileMode.Create);

        var output = new MemoryStream();

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

        var bytes = output.ToArray();
//        output.Flush();
        output.Close();
        input.Flush();
        input.Close();

        return bytes;

    }

}
