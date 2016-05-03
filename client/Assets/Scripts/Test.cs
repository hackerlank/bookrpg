using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using book.rpg;
using bookrpg.log;
using bookrpg.resource;
using ICSharpCode.SharpZipLib.Zip;

public class Test : MonoBehaviour
{

    string baseUrl = "/Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";
    string baseUrlFile = "file:///Users/llj/Downloads/arpg/bookrpg/client/Assets/abs/";

    WWW www;

    AssetBundle ab;

    UnityEngine.Object[] textureArr;


    public void  loadAssetBundle()
    {
        string uncompress = baseUrlFile + "map";
        string compress = baseUrlFile + "map";

//        StartCoroutine(doLoadAssetBundle(uncompress));
//        StartCoroutine(doLoadAssetBundle(compress));

//        decompressZLIB();
//        StartCoroutine(decompressUnity());


//        string u1 = "http://127.0.0.1/map";
//        string u2 = "http://127.0.0.1/mapc";
//        StartCoroutine(load(u2));

    }


    IEnumerator load(string url)
    {
        www = new WWW(url);
        yield return www;

        Debug.Log(www.bytes.Length);
    }

    private IEnumerator doLoadAssetBundle(string url)
    {
        www = new WWW(url);

        yield return www;

        Debug.LogFormat("loadAssetBundle: {0}", url);

        ab = www.assetBundle;
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
        Debug.LogFormat("loadTexture: {0}", textureArr.Length.ToString());

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

}
