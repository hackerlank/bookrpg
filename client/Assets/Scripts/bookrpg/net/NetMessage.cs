using UnityEngine;
using System;
using System.Collections;
using System.IO;
using bookrpg.net.protobuf;

namespace bookrpg.net
{
    public class NetMessage : INetMessage, IMessage
    {
        #region head

        public int opcode  { get; set; }

        public ushort route1;

        public ushort route2;

        public int flag;

        #endregion

        public NetMessage()
        {
           
        }

        /// <summary>
        /// Parses message, include body and head
        /// </summary>
        public void Deserialize(byte[] bytes)
        {
            using (var stream = new ByteArray(bytes))
            {
                ParseFrom(stream);
            }
        }

        public void Deserialize(ByteArray stream)
        {
            uint headSize = stream.ReadUInt16();
            var pos = stream.position;
            opcode = stream.ReadInt32();
            route1 = stream.ReadUInt16();
            route2 = stream.ReadUInt16();
            flag = stream.ReadInt32();
            var headEnd = stream.position;
            stream.position = pos + headSize;
            ParseFrom(stream);
        }

        /// <summary>
        /// Serialize message, include body and head
        /// </summary>
        public byte[] Serialize()
        {
            using (ByteArray stream = new ByteArray(), head = new ByteArray())
            {
                head.Write(opcode);
                head.Write(route1);
                head.Write(route2);
                head.Write(flag);
                var bytes = head.ToArray();
                stream.Write((ushort)bytes.Length);
                stream.Write(bytes);
                WriteTo(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Parses message body, not include head, used by protobuf
        /// </summary>
        public virtual void ParseFrom(ByteArray stream)
        {
            
        }

        /// <summary>
        /// Serialize message body, not include head, used by protobuf
        /// </summary>
        public virtual void WriteTo(ByteArray stream)
        {
            
        }
    }
}
