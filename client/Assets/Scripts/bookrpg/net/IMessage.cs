using UnityEngine;
using System.IO;
using System.Collections;
using bookrpg.utils;

namespace bookrpg.net
{
    public interface IMessage
    {
        uint opcode{ get; set; }

        void Deserialize(byte[] value);

        void Deserialize(ByteArray value);

        byte[] Serialize();
    }
}
