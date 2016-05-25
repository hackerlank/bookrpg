using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;
using bookrpg.core;

namespace bookrpg.mgr
{
    public class TickMgr
    {
        private class TickItem
        {
            public float time;
            public int frame;
            public BKAction action;

            private float last;

            public TickItem(float time, BKAction action)
            {
                this.time = time;
                this.action = action;
                last = Time.time;
            }

            public TickItem(int frame, BKAction action)
            {
                this.frame = frame;
                this.action = action;
                last = 0f;
            }

            public void Update()
            {
                if (time > 0f)
                {
                    if (Time.time - last >= time)
                    {
                        action.Invoke();
                        last = Time.time;
                    }
                } else
                {
                    if (last >= frame)
                    {
                        action.Invoke();
                        last = 0f;
                    } else
                    {
                        last++;
                    }
                }
            }
        }

        public static BKEvent onFixedUpdate = new BKEvent();
        public static BKEvent onUpdate = new BKEvent();
        public static BKEvent onLateUpdate = new BKEvent();
        public static BKEvent onPerSecond = new BKEvent();

        private static float lastTime;

        private static int intervalId = 1;

        private static Dictionary<int, TickItem> intervals = new Dictionary<int, TickItem>();


        public static int SetInterval(float timeInterval, BKAction action)
        {
            if (action == null)
            {
                return 0;
            }

            if (timeInterval <= 0f)
            {
                action.Invoke();
                timeInterval = 0.1f;
            }

            while (intervals.ContainsKey(intervalId))
            {
                intervalId++;
                if (intervalId == int.MaxValue)
                {
                    intervalId = 1;
                }
            }

            intervals.Add(intervalId, new TickItem(timeInterval, action));
            return intervalId;
        }

        public static int SetInterval(int frameInterval, BKAction action)
        {
            if (action == null)
            {
                return 0;
            }

            if (frameInterval <= 0)
            {
                action.Invoke();
                frameInterval = 1;
            }

            while (intervals.ContainsKey(intervalId))
            {
                intervalId++;
                if (intervalId == int.MaxValue)
                {
                    intervalId = 1;
                }
            }

            intervals.Add(intervalId, new TickItem(frameInterval, action));
            return intervalId;
        }

        public static void ClearInterval(int intervalId)
        {
            intervals.Remove(intervalId);
        }

        public static void Update()
        {
            if (lastTime == 0f)
            {
                lastTime = Time.time;
            } else if (Time.time - lastTime >= 1f)
            {
                onPerSecond.Invoke();
                lastTime = Time.time;
            }

            foreach (var item in intervals.Values)
            {
                item.Update();
            }
        }
    }
}
