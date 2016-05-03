using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using book.rpg;
using bookrpg.log;
using bookrpg.resource;

public class Test2 : MonoBehaviour {

    BatchLoader bl;

	// Use this for initialization
	void Start () {

        LoaderMgr.init(true);

        LoaderMgr.maxLoadingCount = 1;

        string u1 = "http://127.0.0.1/map";
        string u2 = "http://127.0.0.1/mapc";

        StartCoroutine(load(u2));

        return;


        string[] urls = new string[]
        {
            "map",
            "abs",
            "php.ini",
            "1.php",
            "2.php",
            "mapc",
        };

        bl = LoaderMgr.batchLoad();

        bl.addLoader("map", 0, (int)(11.2 * 1024 * 1024));
        bl.addLoader("1.php", 0, 500, 100);
        bl.addLoader("2.php", 0, 500);
        bl.addLoader("1.php", 0, 500);
        bl.addLoader("mapc", 1, (int)(7.6 * 1024 * 1024));

        bl.baseUrl = "http://127.0.0.1";
        bl.backupBaseUrl = "http://localhost";

        bl.onOneComplete += (ld) =>
        {
            Debug.LogFormat("url:{0}, bytes:{1}", ld.url, ld.bytesLoaded);
        };

        bl.onComplete += (ld2) =>
        {
            Debug.LogFormat("total:{0}, bytes:{1}, error:{2}", 
                ld2.bytesTotal, ld2.bytesLoaded, ld2.errorCount);
        };

	}
	
	// Update is called once per frame
	void Update () {
        //LoaderMgr.update();

//        if(bl != null && !bl.isComplete)
//        Debug.LogErrorFormat("loaded:{0}, total:{1}, progress:{2}", bl.bytesLoaded, bl.bytesTotal, bl.progress);
	}


    private WWW www;

    IEnumerator load(string url)
    {
        www = new WWW(url);
        yield return www;

        Debug.Log(www.bytes.Length);
    }
}
