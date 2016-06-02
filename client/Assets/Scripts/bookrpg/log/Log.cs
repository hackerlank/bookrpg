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
        ALL = 0,
        FATAL = 1,
        ERROR = 2,
        WARNING = 3,
        INFO = 4,
        DEBUG = 5,
    }

    public static class Log
    {
        private class LogItem
        {
            public DateTime time { get; private set; }

            public string tag { get; private set; }

            public LogLevel level { get; private set; }

            public string message { get; private set; }

            private string targetMessage = null;

            public LogItem(DateTime time, LogLevel level, string tag, string message)
            {
                this.time = time;
                this.level = level;
                this.tag = tag;
                this.message = message;
            }

            public override string ToString()
            {
                if (targetMessage == null)
                {
                    targetMessage = string.Format("[{0}] [{1}] {2}", 
                        time.ToString("yyyy-MM-dd HH:mm:ss"),
                        tag == null ? string.Empty : tag,
                        levelToString(level)
                    );
                }
                return targetMessage;
            }

            private string levelToString(LogLevel lv)
            {
                switch (lv)
                {
                    case LogLevel.FATAL:
                        return "fatal";
                    case LogLevel.ERROR:
                        return "error";
                    case LogLevel.WARNING:
                        return "warning";
                    case LogLevel.INFO:
                        return "info";
                    case LogLevel.DEBUG:
                        return "debug";
                    default:
                        return string.Empty;
                }
            }
        }

        private static Dictionary<string, List<LogItem>> logItems = new Dictionary<string, List<LogItem>>();

        private static bool ignore;

        public static int maxLogCache = 4096;

        public static bool isFatalLEnabled = true;
        public static bool isErrorEnabled = true;
        public static bool isWarningEnabled = true;
        public static bool isInfoEnabled = true;
        public static bool isDebugEnabled = true;

        public static void Init()
        {
            Application.logMessageReceived += (condition, stackTrace, type) =>
            {
                if (!ignore)
                {

                    //                Console.WriteLine(condition);
                }

                ignore = false;
            };
        }

        public static void Fatal(object message)
        {
            if (isFatalLEnabled)
            {
                WriteLog(LogLevel.FATAL, message);
            }
        }

        public static void Fatal(string tag, object message)
        {
            if (isFatalLEnabled)
            {
                WriteLog(LogLevel.FATAL, tag, message);
            }
        }

        public static void FatalFormat(string format, params object[] args)
        {
            if (isFatalLEnabled)
            {
                WriteLog(LogLevel.FATAL, String.Format(format, args));
            }
        }

        public static void Error(object message)
        {
            if (isErrorEnabled)
            {
                WriteLog(LogLevel.ERROR, message);
            }
        }

        public static void Error(string tag, object message)
        {
            if (isErrorEnabled)
            {
                WriteLog(LogLevel.ERROR, tag, message);
            }
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            if (isErrorEnabled)
            {
                WriteLog(LogLevel.ERROR, String.Format(format, args));
            }
        }

        public static void Warning(object message)
        {
            if (isWarningEnabled)
            {
                WriteLog(LogLevel.WARNING, message);
            }
        }

        public static void Warning(string tag, object message)
        {
            if (isWarningEnabled)
            {
                WriteLog(LogLevel.WARNING, tag, message);
            }
        }

        public static void WarningFormat(string format, params object[] args)
        {
            if (isWarningEnabled)
            {
                WriteLog(LogLevel.WARNING, String.Format(format, args));
            }
        }

        public static void Info(object message)
        {
            if (isInfoEnabled)
            {
                WriteLog(LogLevel.INFO, message);
            }
        }

        public static void Info(string tag, object message)
        {
            if (isInfoEnabled)
            {
                WriteLog(LogLevel.INFO, tag, message);
            }
        }

        public static void InfoFormat(string format, params object[] args)
        {
            if (isInfoEnabled)
            {
                WriteLog(LogLevel.INFO, String.Format(format, args));
            }
        }

        public static void Debug(object message)
        {
            if (isDebugEnabled)
            {
                WriteLog(LogLevel.DEBUG, message);
            }
        }

        public static void Debug(string tag, object message)
        {
            if (isDebugEnabled)
            {
                WriteLog(LogLevel.DEBUG, tag, message);
            }
        }

        public static void DebugFormat(string format, params object[] args)
        {
            if (isDebugEnabled)
            {
                WriteLog(LogLevel.DEBUG, String.Format(format, args));
            }
        }

        public static string[] GetLogs(string tag, LogLevel level = LogLevel.ALL)
        {
            var strArr = new List<string>();

            if (logItems.ContainsKey(tag))
            {
                foreach (var item in logItems[tag])
                {
                    if (item.level == LogLevel.ALL || item.level == level)
                    {
                        strArr.Add(item.ToString());
                    }
                }
            }

            return strArr.ToArray();
        }

        public static void WriteLog(LogLevel level, object message)
        {
            WriteLog(level, message);
        }

        //[2016-02-05 08:12:37] [tag error] xxxx in stack trace
        public static void WriteLog(LogLevel level, string tag, object message)
        {
            LogItem item;

            switch (level)
            {
                case LogLevel.FATAL:
                    //--4>TODO: 发布后 error 也会导致游戏崩溃/退出吗? 如果是, 那取消所有 ERROR
                    if (isFatalLEnabled)
                    {
                        item = new LogItem(DateTime.Now, level, tag, message.ToString());
                        AddLog(item);
                        ignore = true;
                        UnityEngine.Debug.LogError(item.ToString());
                    }
                    break;
                case LogLevel.ERROR:
                    if (isErrorEnabled)
                    {
                        item = new LogItem(DateTime.Now, level, tag, message.ToString());
                        AddLog(item);
                        ignore = true;
                        UnityEngine.Debug.LogError(item.ToString());
                    }
                    break;
                case LogLevel.WARNING:
                    if (isWarningEnabled)
                    {
                        item = new LogItem(DateTime.Now, level, tag, message.ToString());
                        AddLog(item);
                        ignore = true;
                        UnityEngine.Debug.LogWarning(item.ToString());
                    }
                    break;
                case LogLevel.INFO:
                    if (isInfoEnabled)
                    {
                        item = new LogItem(DateTime.Now, level, tag, message.ToString());
                        AddLog(item);
                        ignore = true;
                        UnityEngine.Debug.Log(item.ToString());
                    }
                    break;
                case LogLevel.DEBUG:
                    if (isDebugEnabled)
                    {
                        item = new LogItem(DateTime.Now, level, tag, message.ToString());
                        AddLog(item);
                        ignore = true;
                        UnityEngine.Debug.Log(item.ToString());
                    }
                    break;
            }
        }

        private static void AddLog(LogItem item)
        {
            List<LogItem> list;
            if (!logItems.ContainsKey(item.tag))
            {
                list = new List<LogItem>();
                logItems.Add(item.tag, list);
            } else
            {
                list = logItems[item.tag];
            }
            list.Add(item);
            if (list.Count > maxLogCache)
            {
                list.RemoveAt(0);
            }
        }
    }
}