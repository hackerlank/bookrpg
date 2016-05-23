using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using UnityEngine;
using bookrpg.resource;

namespace bookrpg.resource
{
    public class ResourcePackImpl : IResourcePack
    {
        public string srcFile
        {
            get;
            private set;
        }

        public string targetFile
        {
            get;
            private set;
        }

        public int size
        {
            get;
            private set;
        }

        public int version
        {
            get;
            private set;
        }

        public string[] resources
        {
            get;
            private set;
        }

        public string[] dependencies
        {
            get;
            private set;
        }

        public bool beDependent
        {
            get;
            private set;
        }

        public bool singleDirectResource
        {
            get{ return !beDependent && (resources == null || resources.Length < 2); }
        }

        public uint crc
        {
            get;
            private set;
        }

        public string packType
        {
            get;
            private set;
        }

        public string encryption
        {
            get;
            private set;
        }

        public string compression
        {
            get;
            private set;
        }

        public object customData
        {
            get;
            set;
        }

        public void fromJson(JsonData data)
        {
            var keys = data.Keys;

            srcFile = (string)data["srcFile"];
            targetFile = (string)data["targetFile"];
            size = (int)data["size"];
            version = (int)data["version"];

            if (keys.Contains("resources"))
            {
                var res = data["resources"];
                resources = new string[res.Count];
                for (int i = 0; i < res.Count; i++)
                {
                    resources[i] = (string)res[i];
                }
            }

            if (keys.Contains("dependencies"))
            {
                var deps = data["dependencies"];
                dependencies = new string[deps.Count];
                for (int i = 0; i < deps.Count; i++)
                {
                    dependencies[i] = (string)deps[i];
                }
            }

            if (keys.Contains("beDependent"))
            {
                beDependent = (bool)data["beDependent"];
            }

            if (keys.Contains("crc"))
            {
                crc = (uint)data["crc"];
            }

            if (keys.Contains("packType"))
            {
                packType = (string)data["packType"];
            }

            if (keys.Contains("encryption"))
            {
                encryption = (string)data["encryption"];
            }

            if (keys.Contains("compression"))
            {
                compression = (string)data["compression"];
            }
        }

        public string toJson()
        {
            var jw = new JsonWriter();
            jw.WriteObjectStart();
            jw.PrettyPrint = true;

            jw.WritePropertyName("srcFile");
            jw.Write(srcFile);

            jw.WritePropertyName("targetFile");
            jw.Write(targetFile);

            jw.WritePropertyName("size");
            jw.Write(size);

            jw.WritePropertyName("version");
            jw.Write(version);

            jw.WritePropertyName("beDependent");
            jw.Write(beDependent);

            if (crc > 0)
            {
                jw.WritePropertyName("crc");
                jw.Write(crc);
            }

            if (!string.IsNullOrEmpty(packType))
            {
                jw.WritePropertyName("packType");
                jw.Write(packType);
            }

            if (!string.IsNullOrEmpty(compression))
            {
                jw.WritePropertyName("encryption");
                jw.Write(encryption);
            }

            if (!string.IsNullOrEmpty(compression))
            {
                jw.WritePropertyName("compression");
                jw.Write(compression);
            }

            if (resources != null && resources.Length > 0)
            {
                jw.WritePropertyName("resources");
                jw.WriteArrayStart();
                foreach (var item in resources)
                {
                    jw.Write(item);
                }
                jw.WriteArrayEnd();
            }

            if (dependencies != null && dependencies.Length > 0)
            {
                jw.WritePropertyName("dependencies");
                jw.WriteArrayStart();
                foreach (var item in dependencies)
                {
                    jw.Write(item);
                }
                jw.WriteArrayEnd();
            }

            jw.WriteObjectEnd();

            return jw.ToString();
        }
    }
}
