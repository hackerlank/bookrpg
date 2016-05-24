using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace bookrpg.Editor
{
    [Serializable]
    public class PackProject
    {
        public List<string> pack;

        public List<string> exclude;

        public Dictionary<string, List<string>> bulk;

        public PackProject()
        {
            pack = new List<string>();
            exclude = new List<string>();
            bulk = new Dictionary<string, List<string>>();
        }

        public string ToJson()
        {
            return JsonMapper.ToJson(this);
        }

        public static PackProject FromJson(string value)
        {
            return JsonMapper.ToObject<PackProject>(value);
        }

    }
}
