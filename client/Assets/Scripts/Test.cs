using UnityEngine;
using System.Collections;
using System;
using System.IO;
using book.rpg;
using bookrpg.log;

public class Test : MonoBehaviour {

    WeakReference weak;

	// Use this for initialization
    void Start () {

        var ab = load();

        GC.Collect();

        Debug.Log(ab.GetInstanceID());
	
	}


	// Update is called once per frame
	void Update () {
	
	}

    AssetBundle load () {

        WWW www = new WWW("file://" + Application.dataPath + "/abs/abs");

        while (!www.isDone)
        {
        }

        weak = new WeakReference(www);

        return www.assetBundle;
    }
}
