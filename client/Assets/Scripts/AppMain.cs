using UnityEngine;
using System.Collections;
using bookrpg.core;
using bookrpg.mgr;
using bookrpg.log;

public class AppMain : MonoBehaviour
{

    public string txt;

    // Use this for initialization
    void Awake()
    {
        Debug.Log("Init AppMain:" + txt);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Init()
    {
        
    }
}
