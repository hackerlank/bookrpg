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

        public string Serialize()
        {
            return JsonMapper.ToJson(this);
        }

        public static PackProject ParseFrom(string json)
        {
            return JsonMapper.ToObject<PackProject>(json);
        }

    }
}
