using UnityEngine;
using System.Collections;

namespace bookrpg.net
{
    public interface INetMessageBuilder
    {
        INetMessage BuilderMessage(int opcode);

        INetMessage BuilderMessage(ByteArray stream);
    }
}
