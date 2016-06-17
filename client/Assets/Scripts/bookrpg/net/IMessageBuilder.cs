using UnityEngine;
using System.Collections;
using bookrpg.utils;

namespace bookrpg.net
{
    public interface IMessageBuilder
    {
        IMessage BuildMessage(uint opcode);

        IMessage BuildMessage(ByteArray stream);
    }
}
