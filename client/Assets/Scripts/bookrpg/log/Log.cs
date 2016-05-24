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
        ALL,
        ERROR,
        WARNING,
        INFO,
        DEBUG,
    }

    public static class Log
    {
        private static Dictionary<string, List<string>> tagLogs = new Dictionary<string, List<string>>();

        public static bool showTagLogInConsole = true;
        public static int maxTagLogCount = 4096;

        public static bool isErrorEnabled = true;
        public static bool isWarningEnabled = true;
        public static bool isInfoEnabled = true;
        public static bool isDebugEnabled = true;

        public static void Init()
        {
            Application.logMessageReceived += (condition, stackTrace, type) =>
            {
//                Console.WriteLine(condition);
            };
        }

        public static void Error(string format, params object[] args)
        {
            DoLog(LogLevel.ERROR, String.Format(format, args));
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            DoLog(LogLevel.ERROR, String.Format(format, args));
        }

        public static void Warning(string format, params object[] args)
        {
            DoLog(LogLevel.WARNING, String.Format(format, args));
        }

        public static void Info(string format, params object[] args)
        {
            DoLog(LogLevel.INFO, String.Format(format, args));
        }

        public static void Debug(string format, params object[] args)
        {
            if (isDebugEnabled)
            {
                DoLog(LogLevel.DEBUG, String.Format(format, args));
            }
        }

        /// <summary>
        /// collect similar infomation and debug it, e.g. net msg, config init
        /// </summary>
        public static void AddTagLog(string tag, string format, params object[] args)
        {
            if (!isDebugEnabled)
            {
                return;
            }

            string str = String.Format(format, args);
            str = DateTime.Now.ToLongTimeString() + " " + str;
            if (showTagLogInConsole)
            {
               UnityEngine.Debug.Log(str);
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

        public static void ClearTagLog(string tag)
        {
            if (!isDebugEnabled)
            {
                return;
            }

            lock (tagLogs)
            {
                if (tagLogs.ContainsKey(tag))
                {
                    tagLogs[tag].Clear();
                    tagLogs.Remove(tag);
                }
            }
        }

        public static string GetLogs(string tag, LogLevel level = LogLevel.ALL)
        {
            string[] strArr = null;

            if (tagLogs.ContainsKey(tag))
            {
                strArr = tagLogs[tag].ToArray();
            }

            return strArr != null ? string.Join("\r\n", strArr) : string.Empty;
        }

        public static void DoLog(LogLevel level, object message)
        {
            doLog(level, message);
        }

        public static void DoLog(LogLevel level, string tag, object message)
        {
            doLog(level, message, tag);
        }

        private static void doLog(LogLevel level, object message, string tag = null)
        {
            switch (level)
            {
                case LogLevel.ERROR:
                //--4>TODO: 发布后 error 也会导致游戏崩溃/退出吗? 如果是, 那取消所有 ERROR
                    if (isErrorEnabled)
                    {
                        UnityEngine.Debug.LogError(message);
                    }
                    break;
                case LogLevel.WARNING:
                    if (isWarningEnabled)
                    {
                        UnityEngine.Debug.LogWarning(message);
                    }
                    break;
                case LogLevel.INFO:
                    if (isInfoEnabled)
                    {
                        UnityEngine.Debug.Log(message);
                    }
                    break;
                case LogLevel.DEBUG:
                    if (isDebugEnabled)
                    {
                        UnityEngine.Debug.Log(message);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}