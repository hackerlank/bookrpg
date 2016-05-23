using System.Collections;
using System.Collections.Generic;

namespace bookrpg.resource
{
    public interface IResourceFile
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        int number{ get; }

        /// <summary>
        /// 资源名称
        /// </summary>
        string name{ get; }

        /// <summary>
        /// 资源类型
        /// </summary>
        string type{ get; }

        /// <summary>
        /// 资源所对应的文件
        /// </summary>
        string srcFile{ get; }

        /// <summary>
        /// 资源实际对应的文件，因为重新命名或打包。
        /// 比如用hash重新命名，用"原文件名+版本"重命名
        /// </summary>
        string targetFile{ get; }

        /// <summary>
        /// 资源大小 bytes
        /// </summary>
        int size{ get; }

        /// <summary>
        /// 版本
        /// </summary>
        int version { get; }

        /// <summary>
        /// 依赖的其它资源
        /// </summary>
        string[] dependencies{ get; }

        /// <summary>
        /// 是否被其它资源依赖
        /// </summary>
        bool beDependent{ get; }

        /// <summary>
        /// 目标文件的crc32
        /// </summary>
        uint crc{ get; }

        /// <summary>
        /// 打包类型：zip、7z、AssetBundle...
        /// </summary>
        string packType{ get; }

        /// <summary>
        /// 资源在包中的标识，用于从包中取出资源
        /// </summary>
        string idInPack{ get; }

        /// <summary>
        /// 包中是否只有一个直接使用的资源，也就是使用的时候只会用AssetBundle.LoadAsset()
        /// 加载其中的一个资源，其它的资源都是间接被加载的。
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
    }
}
