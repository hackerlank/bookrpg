using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_WEBPLAYER
#elif UNITY_WEBGL
#else
using System.IO;
#endif

namespace bookrpg.resource
{
    public class ResourceTableImpl : IResourceTable
    {
        private List<ResourcePackImpl> resourcePacks;
        private Dictionary<string, ResourcePackImpl> resources;

        private string savePath;

        public void load(string path)
        {
            this.savePath = path;
        }

        public void save(string path = null)
        {
            if (path != null)
            {
                this.savePath = path;
            }

            var list = new List<string>();
            list.Add("[");
            foreach (var item in resourcePacks)
            {
                list.Add(item.toJson() + ",");
            }
            list.Add("]");

            var str = string.Join("\n", list.ToArray());

#if UNITY_WEBPLAYER
#elif UNITY_WEBGL
#else
            File.WriteAllText(savePath, str);
#endif
        }

        public IResourceFile getResourceFile(string resourcePath)
        {
            throw new System.NotImplementedException();
        }

        public IResourceFile getResourceFile(int resourceNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
