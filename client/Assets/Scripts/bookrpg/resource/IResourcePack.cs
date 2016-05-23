using System.Collections;
using System.Collections.Generic;

namespace bookrpg.resource
{
    public interface IResourcePack
    {
        /// <summary>
        /// 资源包所对应的文件
        /// </summary>
        string srcFile{ get; }

        /// <summary>
        /// 资源包实际对应的文件，因为重新命名，
        /// 比如用hash重新命名，用"原文件名+版本"重命名
        /// </summary>
        string targetFile{ get; }

        /// <summary>
        /// 资源包大小 bytes
        /// </summary>
        int size{ get; }

        /// <summary>
        /// 版本
        /// </summary>
        int version { get; }

        /// <summary>
        /// 包含的资源
        /// </summary>
        string[] resources { get; }

        /// <summary>
        /// 依赖的其它资源包
        /// </summary>
        string[] dependencies{ get; }

        /// <summary>
        /// 是否被其它资源包依赖
        /// </summary>
        bool beDependent{ get; }

        /// <summary>
        /// crc32
        /// </summary>
        uint crc{ get; }

        /// <summary>
        /// 打包类型：zip、7z、AssetBundle...
        /// </summary>
        string packType{ get; }

        /// <summary>
        /// 包中是否只有一个直接使用的资源包，也就是使用的时候只会用AssetBundle.LoadAsset()
        /// 加载其中的一个资源包，其它的资源包都是间接被加载的。看资源数量即可。
        /// </summary>
        bool singleDirectResource{ get; }

        /// <summary>
        /// 加密器
        /// </summary>
        string encryption{ get; }

        /// <summary>
        /// 压缩器
        /// </summary>
        string compression{ get; }


        object customData{ get; set; }
    }
}
