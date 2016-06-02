using UnityEngine;
using System.IO;
using System.Collections;

namespace bookrpg.net
{
    public interface INetMessage
    {
        int opcode{ get; set; }

        void Deserialize(byte[] value);

        void Deserialize(ByteArray value);

        byte[] Serialize();
    }
}
