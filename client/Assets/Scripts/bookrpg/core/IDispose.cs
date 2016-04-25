using System;

namespace bookrpg.core
{
    public interface IDispose : IDisposable
    {
        bool hasDisposed();
    }
}
