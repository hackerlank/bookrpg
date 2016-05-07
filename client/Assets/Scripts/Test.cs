using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using bookrpg.log;
using bookrpg.core;
using bookrpg.resource;
using ICSharpCode.SharpZipLib.Zip;

public class Test : MonoBehaviour
{

    string baseUrl = "/Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";
    string baseUrlFile = "file:///Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";

    WWW www;

    AssetBundle ab;
    AssetBundle ab2;

    UnityEngine.Object[] textureArr;

    LocalResMgr mgr;


    public void  loadAssetBundle()
    {
        string uncompress = baseUrlFile + "scene";
        string compress = baseUrlFile + "scenec";
        string compress2 = baseUrlFile + "p2";

//        StartCoroutine(doLoadAssetBundle(uncompress));
//        StartCoroutine(doLoadAssetBundle(compress));


        mgr = new LocalResMgr();
        mgr.init(new rest());

        ResourceMgr.init(mgr);


        ResourceMgr.load(uncompress, url => {
            var ab = ResourceMgr.getResource(url);
            Debug.Log(ResourceMgr.hasResource(uncompress));
        });


//        decompressZLIB();
//        StartCoroutine(decompressUnity());


//        string u1 = "http://127.0.0.1/map";
//        string u2 = "http://127.0.0.1/mapc";
//        StartCoroutine(load(u2));

    }


    IEnumerator load(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        ab2 = www.assetBundle;
        Debug.Log(www.url);
    }

    private IEnumerator doLoadAssetBundle(string url)
    {
        var t1 = Time.time;
        www = new WWW(url);

        yield return www;

        Debug.LogFormat("loadAssetBundle: {0}, time: {1}", url, Time.time - t1);

        ab = www.assetBundle;

//        ab.Unload(true);

        www.Dispose();
        www = null;

       
    }

    public void loadTexture()
    {
        if (ab == null)
        {
            var bytes = decompressLZMA();
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

        foreach (var obj in textureArr)
        {
//            Instantiate(obj);
        }

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

    public void disposeTexture()
    {
        for (int i = 0; i < textureArr.Length; i++)
        {
            textureArr[i] = null;
        }

        Resources.UnloadUnusedAssets();

        GC.Collect();
    }

    public Text txt;

    private byte[] decompressLZMA()
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

    private void decompressZLIB()
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

    private IEnumerator decompressUnity()
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

        LoaderMgr.init();

//        LoaderMgr.behaviour = this;
//        LoaderMgr.timeout = 3f;
//        LoaderMgr.baseUrl = "http://127.0.0.1/";
//        LoaderMgr.backupBaseUrl = "http://localhost/";

    }


    void Update()
    {

//        LoaderMgr.update();

        if (www != null)
        {


        }


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

    class rest : IResourceTable
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
