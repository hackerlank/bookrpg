using UnityEngine;
using System.Collections;
using bookrpg.mgr;
using bookrpg.resource;

namespace bookrpg.mgr
{
    public class Bookrpg : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Init Bookrpg");
            DontDestroyOnLoad(this);
            CoroutineMgr.Init(this);
        }

        void FixedUpdate()
        {
            TickMgr.onFixedUpdate.Invoke();
        }

        void Update()
        {
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
