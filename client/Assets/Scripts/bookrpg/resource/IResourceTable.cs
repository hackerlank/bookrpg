using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.resource
{

    public interface IResourceTable : ISerialize
    {
        IResourceFile GetResourceFile(string resourcePath);

        IResourceFile GetResourceFile(int resourceNumber);

        IDictionary<string, IResourcePack> resourcePackList { get;}

        void Save(string path = null);
    }
}
