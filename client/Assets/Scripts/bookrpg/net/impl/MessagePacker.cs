using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using bookrpg.net.protobuf;
using bookrpg.utils;

namespace bookrpg.net
{
    public class MessagePacker : IMessagePacker
    {
        private Dictionary<uint, Type> parsers = new Dictionary<uint, Type>();

        private Message msg = new Message();

        public void addMessageParser<T>(uint opcode) where T : IMessage
        {
            if (!parsers.ContainsKey(opcode))
            {
                parsers.Add(opcode, typeof(T));
            } else
            {
                Debug.LogWarningFormat("repeated MessageParser, opcode: {0}", opcode);
                parsers[opcode] = typeof(T);
            }
        }

        public IMessage CreateMessage(uint opcode)
        {
            Type parser;
            if (parsers.TryGetValue(opcode, out parser))
            {
                return (IMessage)System.Activator.CreateInstance(parser);
            }
            return null;
        }

        public byte[] PackMessage(IMessage message, bool useBigEndian = false)
        {
            using (var byteArray = new ByteArray())
            {
                byteArray.endian = useBigEndian ? Endian.BIG_ENDIAN : Endian.LITTLE_ENDIAN;
                return message.Serialize(byteArray).ToArray();
            }
        }

        public IMessage UnpackMessage(byte[] bytes, bool useBigEndian = false)
        {
            using (var byteArray = new ByteArray(bytes))
            {
                byteArray.endian = useBigEndian ? Endian.BIG_ENDIAN : Endian.LITTLE_ENDIAN;
                return UnpackMessage(byteArray);
            }
        }

        public IMessage UnpackMessage(ByteArray byteArray)
        {
            msg.opcode = 0;
            msg.Deserialize(byteArray);
            var message = CreateMessage(msg.opcode);
            if (message != null)
            {
                byteArray.position = 0;
                message.Deserialize(byteArray);
                return message;
            } else
            {
                Debug.LogWarningFormat("unknown opcode: {0}", msg.opcode);
                return null;
            }
        }
    }
}
