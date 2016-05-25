using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace bookrpg.mgr
{
    public class CoroutineBehaviour : MonoBehaviour
    {
    }

    public static class CoroutineMgr
    {
        private static MonoBehaviour globalBehaviour;
        private static CoroutineBehaviour sceneBehaviour;

        public static void Init(MonoBehaviour behaviour)
        {
            globalBehaviour = behaviour;
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine, false);
        }

        public static Coroutine StartCoroutine(IEnumerator routine, bool destroyWhenChangeLevel)
        {
            //use global MonoBehaviour exe coroutine
            if (!destroyWhenChangeLevel && globalBehaviour != null)
            {
                return globalBehaviour.StartCoroutine(routine);
            }
            //用当前场景内的MonoBehavior执行协成
            if (sceneBehaviour == null)
            {
                sceneBehaviour = new GameObject("CoroutineManager").AddComponent<CoroutineBehaviour>();
            }
            return sceneBehaviour.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(string methodName, object value = null)
        {
            return StartCoroutine(methodName, false, value);
        }

        public static Coroutine StartCoroutine(string methodName, bool destroyWhenChangeLevel, object value = null)
        {
            if (!destroyWhenChangeLevel && globalBehaviour != null)
            {
                return globalBehaviour.StartCoroutine(methodName, value);
            }
            if (sceneBehaviour == null)
            {
                sceneBehaviour = new GameObject("CoroutineManager").AddComponent<CoroutineBehaviour>();
            }
            return sceneBehaviour.StartCoroutine(methodName, value);
        }

        public static void StopCoroutine(string routine)
        {
            if (globalBehaviour != null)
            {
                globalBehaviour.StopCoroutine(routine);
            }
            if (sceneBehaviour != null)
            {
                sceneBehaviour.StopCoroutine(routine);
            }
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            if (globalBehaviour != null)
            {
                globalBehaviour.StopCoroutine(routine);
            }
            if (sceneBehaviour != null)
            {
                sceneBehaviour.StopCoroutine(routine);
            }
        }

        public static void StopAllCoroutinesInScene(IEnumerator routine)
        {
            if (sceneBehaviour != null)
            {
                sceneBehaviour.StopAllCoroutines();
            }
        }
    }
}
