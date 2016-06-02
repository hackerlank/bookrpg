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

        public int route1;

        public int route2;

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
            uint headSize = ReadUtils.Read_TYPE_UINT32(stream);
            var pos = stream.position;
            opcode = ReadUtils.Read_TYPE_SINT32(stream);
            route1 = ReadUtils.Read_TYPE_SINT32(stream);
            route2 = ReadUtils.Read_TYPE_SINT32(stream);
            flag = ReadUtils.Read_TYPE_SINT32(stream);
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
                WriteUtils.Write_TYPE_SINT32(head, opcode);
                WriteUtils.Write_TYPE_SINT32(head, route1);
                WriteUtils.Write_TYPE_SINT32(head, route2);
                WriteUtils.Write_TYPE_SINT32(head, flag);
                WriteUtils.Write_TYPE_BYTES(stream, head.ToArray());
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
