using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

#if UNITY_WEBPLAYER
#elif UNITY_WEBGL
#else
using System.IO;
#endif

namespace bookrpg.resource
{
    public class ResourceTableImpl : IResourceTable
    {
        private List<ResourcePackImpl> resourcePacks = new List<ResourcePackImpl>();
        private Dictionary<string, ResourceFileImpl> resources = new Dictionary<string, ResourceFileImpl>();

        public string Serialize()
        {
            var list = new List<string>();
            list.Add("[");
            foreach (var item in resourcePacks)
            {
                list.Add(item.ToJson() + ",");
            }
            list.Add("]");

            return string.Join("\n", list.ToArray());
        }

        public bool Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("ResourceTableImpl.deserialize, empty value");
                return false;
            }

            resourcePacks.Clear();
            resources.Clear();

            try
            {
                var data = JsonMapper.ToObject(value);
                for (int i = 0; i < data.Count; i++)
                {
                    var pack = new ResourcePackImpl();
                    pack.FromJson(data[i]);
                    resourcePacks.Add(pack);

                    foreach (var item in pack.resources)
                    {
                        var res = new ResourceFileImpl(pack);
                        res.srcFile = item;
                        resources.Add(item, res);
                    }
                }
                return true;
            } catch (Exception e)
            {
                Debug.LogError("ResourceTableImpl.deserialize, " + e.Message);
                return false;
            }
        }

        public void Save(string path = null)
        {
            if (path == null)
            {
                return;
            }

#if UNITY_WEBPLAYER
#elif UNITY_WEBGL
#else
            File.WriteAllText(path, Serialize());
#endif
        }

        public IResourceFile GetResourceFile(string resourcePath)
        {
            if (resources.ContainsKey(resourcePath))
            {
                return resources[resourcePath];
            }

            return null;
        }

        public IResourceFile GetResourceFile(int resourceNumber)
        {
            return null;
        }
    }
}
