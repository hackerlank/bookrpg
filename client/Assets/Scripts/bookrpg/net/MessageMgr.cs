using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;
using bookrpg.utils;

namespace bookrpg.net
{
    public static class MessageMgr
    {
        private static Dictionary<uint, BKEvent<IMessage>> messages = new Dictionary<uint, BKEvent<IMessage>>();

        /// <summary>
        /// BKFunc<opcode, IMessage>
        /// </summary>
        public static IMessagePacker messagePacker;

        public static void AddMessageListener(uint opcode, BKAction<IMessage> messageHanlder)
        {
            BKEvent<IMessage> item;
            if (!messages.TryGetValue(opcode, out item))
            {
                item = new BKEvent<IMessage>();
                messages.Add(opcode, item);
            }
            item.AddListener(messageHanlder);
        }

        public static void RemoveMessageListener(uint opcode, BKAction<IMessage> messageHanlder)
        {
            BKEvent<IMessage> item;
            if (messages.TryGetValue(opcode, out item))
            {
                item.RemoveListener(messageHanlder);
            }
        }

        public static void DispatchMessage(IMessage message)
        {
            BKEvent<IMessage> item;
            if (messages.TryGetValue(message.opcode, out item))
            {
                item.Invoke(message);
            }
        }

        public static void DispatchMessage(uint opcode, IMessage message)
        {
            BKEvent<IMessage> item;
            if (messages.TryGetValue(opcode, out item))
            {
                item.Invoke(message);
            }
        }

        public static void addMessagePaser<T>(uint opcode) where T : IMessage
        {
            if (messagePacker != null)
            {
                messagePacker.addMessageParser<T>(opcode);
            }
        }

        public static IMessage CreateMessage(uint opcode)
        {
            if (messagePacker != null)
            {
                return messagePacker.CreateMessage(opcode);
            }

            return null;
        }

        public static byte[] PackMessage(IMessage message, bool useBigEndian = false)
        {
            if (messagePacker != null)
            {
                return messagePacker.PackMessage(message, useBigEndian);
            }

            return null;
        }

        public static IMessage UnpackMessage(byte[] stream, bool useBigEndian = false)
        {
            if (messagePacker != null)
            {
                return messagePacker.UnpackMessage(stream, useBigEndian);
            }

            return null;
        }
    }
}
