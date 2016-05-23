using System.Collections;
using System.Collections.Generic;

namespace bookrpg.resource
{
    public class ResourceFileImpl : IResourceFile
    {
        private IResourcePack pack;

        public ResourceFileImpl(IResourcePack pack)
        {
            this.pack = pack;
        }

        /// <summary>
        /// 资源编号
        /// </summary>
        public int number
        { 
            get { return 0; } 
        }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string name
        { 
            get { return ""; } 
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public string type
        { 
            get { return null; } 
        }

        public string srcFile
        {
            get;
            set;
        }

        public string targetFile
        {
            get { return pack.targetFile; }
        }

        public int size
        {
            get { return pack.size; }
        }

        public int version
        {
            get { return pack.version; }
        }

        public string[] dependencies
        {
            get { return pack.dependencies; }
        }

        public bool beDependent
        {
            get { return pack.beDependent; }
        }

        public bool singleDirectResource
        {
            get { return pack.singleDirectResource; }
        }

        public uint crc
        {
            get { return pack.crc; }
        }

        public string packType
        {
            get { return pack.packType; }
        }

        public string idInPack
        {
            get { return this.srcFile; }
        }

        public string encryption
        {
            get { return pack.encryption; }
        }

        public string compression
        {
            get { return pack.compression; }
        }
    }
}
