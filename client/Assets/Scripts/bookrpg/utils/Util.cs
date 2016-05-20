using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace bookrpg.utils
{

    public class Util
    {
        public static void insertionSort<T>(IList<T> list, Comparison<T> comparison)
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

        public static string getAbsolutePath(string path, string relativePath)
        {
            path = path.Replace('\\', '/');
            path = path.TrimEnd(new char[]{'/'}) + "/";
            relativePath = relativePath.Replace('\\', '/');
            relativePath = relativePath.TrimEnd(new char[]{'/'}) + "/";
            if (!Regex.IsMatch(path, "^(/|[a-zA-Z]:/)"))
            {
                path = relativePath + path;
            }

            return path;
        }


        public static bool wildcardMatch(string path, string wildcard)
        {
            string format = wildcard.Replace("*", ".*").Replace("?", ".?");
            Regex regex = new Regex(format, RegexOptions.IgnoreCase);
            return regex.Match(path).Success;
        }

        /// <summary>
        /// path may: /abc/*d?.txt
        /// </summary>
        public static string[] scanFiles(string path)
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

        public static void deleteFiles(string path)
        {
            foreach (var file in scanFiles(path))
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
    }
}
