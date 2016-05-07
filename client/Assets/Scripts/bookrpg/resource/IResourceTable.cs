using System.Collections;

namespace bookrpg.resource
{

    public interface IResourceTable
    {
        IResourceFile getResourceFile(string resourcePath);

        IResourceFile getResourceFile(int resourceNumber);
    }
}
