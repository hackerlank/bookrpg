using UnityEngine;
using System;
using System.Collections;
using System.IO;
using bookrpg.net.protobuf;
using bookrpg.utils;

namespace bookrpg.net
{
    public class Message : IMessage, IProtobufMessage
    {
        #region head

        private const ushort HEAD_LENGTH = 12;

        public uint opcode  { get; set; }

        public ushort route1;

        public ushort route2;

        public uint flag;

        #endregion

        public Message()
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
            ushort headLength = stream.ReadUInt16();
            var pos = stream.position;
            opcode = stream.ReadUInt32();
            route1 = stream.ReadUInt16();
            route2 = stream.ReadUInt16();
            flag = stream.ReadUInt32();
            var headEnd = stream.position;
            stream.position = pos + headLength;
            ParseFrom(stream);
        }

        /// <summary>
        /// Serialize message, include body and head
        /// </summary>
        public byte[] Serialize()
        {
            using (ByteArray stream = new ByteArray())
            {
                stream.Write(HEAD_LENGTH);
                stream.Write(opcode);
                stream.Write(route1);
                stream.Write(route2);
                stream.Write(flag);
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
