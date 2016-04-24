using UnityEngine;
using System.Collections;
using System;
using System.IO;
using book.rpg;
using bookrpg.log;

public class Test : MonoBehaviour {

    public string Str;

	// Use this for initialization
	void Start () {



        Log.debug("tag", "this is {0}", 5);
        Log.debug("this is {0}", "6");

	
	}

    void log(string a, string b, params object[] args)
    {
        Debug.Log(args.Length);
        Debug.Log("a:" + a);
        Debug.Log(string.Format(b, args));
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void load()
    {
        var form = new WWWForm();
        form.AddField("dd", "vv");
        WWW www = new WWW("http://127.0.0.1/1.php", form);
        while (!www.isDone)
        {
        }

        Debug.Log(www.error);
        Debug.Log(www.text);
    }

    /// <summary>
    /// Test sender, string Msg, int money
    /// </summary>
    public event Action<Test, string, int> onSuccess; 
}
