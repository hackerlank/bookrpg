using UnityEngine;
using System.Collections;
using System;
using System.IO;
using book.rpg;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {

        var txt = File.ReadAllText("Assets/config.json");

        ConfigCfgMgr cc = new ConfigCfgMgr();
        cc.init(txt);

        var c1 = cc.getItem("pos");

        var s = File.ReadAllText("Assets/Sheet.json");
        SheetCfgMgr sc = new SheetCfgMgr();
        sc.init(s, "json");

        var a1 = sc.getItem(1, "武器商");
        var a2 = sc.getItemGroup(1);

        var a3 = sc.getItemByDesc("武器商");
        var a4 = sc.getItemsByDesc("武器商");
        var a5 = sc.getAllItems();

        this.onSuccess += (arg1, arg2, arg3) =>
        {
            Debug.Log(arg2 + ":onSuccess->" + arg3.ToString());
        };

        this.onSuccess(this, "good", 333);

        load();

        Debug.Log(sc);
	
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
