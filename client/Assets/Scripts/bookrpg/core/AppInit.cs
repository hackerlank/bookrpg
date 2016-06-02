using UnityEngine;
using System.Collections;
using bookrpg.mgr;
using bookrpg.resource;
using bookrpg.net;
using bookrpg.log;

namespace bookrpg.core
{
    public class AppInit : MonoBehaviour
    {
        void Awake()
        {
            Log.Init();
            Debug.Log("Init bookrpg Application");
            DontDestroyOnLoad(this);
            CoroutineMgr.Init(this);
        }

        void FixedUpdate()
        {
            TickMgr.onFixedUpdate.Invoke();
        }

        void Update()
        {
            if (RemoteServer.main != null)
            {
                RemoteServer.main.Update();
            }
            if (RemoteServer.chat != null)
            {
                RemoteServer.chat.Update();
            }
            LoaderMgr.Update();
            TickMgr.onUpdate.Invoke();
            TickMgr.Update();
        }

        void LateUpdate()
        {
            TickMgr.onLateUpdate.Invoke();
        }
    }
}
