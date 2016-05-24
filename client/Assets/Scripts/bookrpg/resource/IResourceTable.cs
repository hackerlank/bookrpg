using System.Collections;
using bookrpg.core;

namespace bookrpg.resource
{

    public interface IResourceTable : ISerialize
    {
        IResourceFile GetResourceFile(string resourcePath);

        IResourceFile GetResourceFile(int resourceNumber);

        void Save(string path = null);
    }
}
