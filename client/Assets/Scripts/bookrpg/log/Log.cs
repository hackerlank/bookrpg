///
/// Copyright (c) 2016, bookrpg, All rights reserved.
/// @author llj <wwwllj1985@163.com>
/// @license The MIT License
///

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace bookrpg.log
{

    public enum LogLevel
    {
        ERROR,
        WARNING,
        INFO,
        DEBUG,
    }

    public static class Log
    {
        private static Dictionary<string, List<string>> tagLogs = new Dictionary<string, List<string>>();

        public static bool showTagLogInConsole = false;
        public static int maxTagLogCount = 4096;

        public static void error(string format, params object[] args)
        {
            doLog(LogLevel.ERROR, String.Format(format, args));
        }

        public static void warning(string format, params object[] args)
        {
            doLog(LogLevel.WARNING, String.Format(format, args));
        }

        public static void info(string format, params object[] args)
        {
            doLog(LogLevel.INFO, String.Format(format, args));
        }

        public static void debug(string format, params object[] args)
        {
            doLog(LogLevel.DEBUG, String.Format(format, args));
        }

        /// <summary>
        /// collect similar infomation and debug it, e.g. net msg, config init
        /// </summary>
        public static void addTagLog(string tag, string format, params object[] args)
        {
            string str = String.Format(format, args);
            str = DateTime.Now.ToLongTimeString() + " " + str;
            if (showTagLogInConsole)
            {
                Debug.Log(str);
            }
            lock (tagLogs)
            {
                List<string> list;
                if (!tagLogs.ContainsKey(tag))
                {
                    list = new List<string>();
                    tagLogs.Add(tag, list);
                } else
                {
                    list = tagLogs[tag];
                }
                list.Add(str);
                if (list.Count > maxTagLogCount)
                {
                    list.RemoveAt(0);
                }
            }
        }

        public static void clearTagLog(string tag)
        {
            lock (tagLogs)
            {
                if (tagLogs.ContainsKey(tag))
                {
                    tagLogs[tag].Clear();
                    tagLogs.Remove(tag);
                }
            }
        }

        public static string getTagLog(string tag)
        {
            string[] strArr = null;

            lock (tagLogs)
            {
                if (tagLogs.ContainsKey(tag))
                {
                    strArr = tagLogs[tag].ToArray();
                }
            }

            return strArr != null ? string.Join("\r\n", strArr) : string.Empty;
        }

        //    [Conditional("ENABLE_TRACE")]
        //    public static void TraceEnter(string name)
        //    {
        //        object obj2 = _trace_lock;
        //        lock (obj2)
        //        {
        //            Console.WriteLine("{0} =>{1}", DateTime.Now.ToLongTimeString(), name);
        //        }
        //    }
        //
        //    [Conditional("ENABLE_TRACE")]
        //    public static void TraceLeave(string name)
        //    {
        //        object obj2 = _trace_lock;
        //        lock (obj2)
        //        {
        //            Console.WriteLine("{0} <={1}", DateTime.Now.ToLongTimeString(), name);
        //        }
        //    }

        public static void log(LogLevel level, string format, params object[] args)
        {
            doLog(level, string.Format(format, args));
        }

        private static void doLog(LogLevel level, object message)
        {
            //if(!Application.isEditor)
            //  return;
            switch (level)
            {
                case LogLevel.ERROR:
                //--4>TODO: 发布后 error 也会导致游戏崩溃/退出吗? 如果是, 那取消所有 ERROR
                    Debug.LogError(message);
                    break;
                case LogLevel.WARNING:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.INFO:
                    Debug.Log(message);
                    break;
                case LogLevel.DEBUG:
                //--4>TODO: 以后需要改成判断是否为调试版本
                    Debug.Log(message);
                    break;
                default:
                    break;
            }
        }
    }
}