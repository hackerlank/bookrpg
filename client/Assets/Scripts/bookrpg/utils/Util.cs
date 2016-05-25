using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

#if UNITY_WEBPLAYER
#elif UNITY_WEBGL
#else
using System.IO;
#endif
using System.Text.RegularExpressions;

namespace bookrpg.utils
{

    public class Util
    {
        public static void InsertionSort<T>(IList<T> list, Comparison<T> comparison)
        {
            int count = list.Count;
            for (int i = 1; i < count; i++)
            {
                T y = list[i];
                int num3 = i - 1;
                while ((num3 >= 0) && (comparison(list[num3], y) > 0))
                {
                    list[num3 + 1] = list[num3];
                    num3--;
                }
                list[num3 + 1] = y;
            }
        }

        public static string GetAbsolutePath(string path, string relativePath)
        {
            path = path.Replace('\\', '/');
            path = path.TrimEnd(new char[]{ '/' }) + "/";
            relativePath = relativePath.Replace('\\', '/');
            relativePath = relativePath.TrimEnd(new char[]{ '/' }) + "/";
            if (!Regex.IsMatch(path, "^(/|[a-zA-Z]:/)"))
            {
                path = relativePath + path;
            }

            return path;
        }


        public static bool WildcardMatch(string path, string wildcard)
        {
            string format = wildcard.Replace("*", ".*").Replace("?", ".?");
            Regex regex = new Regex(format, RegexOptions.IgnoreCase);
            return regex.Match(path).Success;
        }

        /// <summary>
        /// path may: /abc/*d?.txt
        /// </summary>
        public static string[] ScanFiles(string path)
        {
            path = path.Replace('\\', '/');

            try
            {
                if (path.IndexOf('*') < 0 && path.IndexOf('?') < 0)
                {
                    if (Directory.Exists(path))
                    {
                        return Directory.GetFiles(path);
                    } else if (File.Exists(path))
                    {
                        return new string[]{ path };
                    }
                }

                var pos = path.LastIndexOf('/');
                string pathDir, wildcardName;
                pathDir = path.Substring(0, pos);
                wildcardName = path.Substring(pos + 1);
                return Directory.GetFiles(pathDir, wildcardName);

            } catch (Exception e)
            {
                Debug.LogError(e.Message);
                return new string[0];
            }
        }

        public static void DeleteFiles(string path)
        {
            foreach (var file in ScanFiles(path))
            {
                try
                {
                    File.Delete(file);
                } catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public static string Load(string path)
        {
            if (path == null)
            {
                Debug.LogWarning("Util.Load failed, path is null");
                return null;
            }

            path = path.Replace("file://", "");

            #if UNITY_WEBPLAYER
            #elif UNITY_WEBGL
            #else
            try
            {
                return File.ReadAllText(path);
            } catch (Exception e)
            {
                Debug.LogError("Util.Load failed: " + e.Message);
                return null;
            }
            #endif
        }

        public static byte[] LoadBytes(string path)
        {
            if (path == null)
            {
                Debug.LogWarning("Util.Load failed, path is null");
                return null;
            }

            path = path.Replace("file://", "");

            #if UNITY_WEBPLAYER
            #elif UNITY_WEBGL
            #else
            try
            {
                return File.ReadAllBytes(path);
            } catch (Exception e)
            {
                Debug.LogError("Util.Load failed: " + e.Message);
                return null;
            }
            #endif
        }

        public static bool Save(string path, string text)
        {
            if (path == null || text == null)
            {
                Debug.LogWarning("Util.Save failed, path or text is null");
                return false;
            }

            #if UNITY_WEBPLAYER
            #elif UNITY_WEBGL
            #else
            try
            {
                path = path.Replace("file://", "");
                File.WriteAllText(path, text);
                return true;
            } catch (Exception e)
            {
                Debug.LogError("Util.Save failed: " + e.Message);
                return false;
            }
            #endif
        }

        public static bool Save(string path, byte[] bytes)
        {
            if (path == null || bytes == null)
            {
                Debug.LogWarning("Util.Save failed, path or bytes is null");
                return false;
            }

            #if UNITY_WEBPLAYER
            #elif UNITY_WEBGL
            #else
            try
            {
                path = path.Replace("file://", "");
                File.WriteAllBytes(path, bytes);
                return true;
            } catch (Exception e)
            {
                Debug.LogError("Util.Save failed: " + e.Message);
                return false;
            }
            #endif
        }
    }
}
