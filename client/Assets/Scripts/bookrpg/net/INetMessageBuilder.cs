using UnityEngine;
using System.Collections;

namespace bookrpg.net
{
    public interface INetMessageBuilder
    {
        INetMessage BuildMessage(int opcode);

        INetMessage BuildMessage(ByteArray stream);
    }
}
