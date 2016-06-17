using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;
using bookrpg.utils;

namespace bookrpg.net
{
    public static class MessageMgr
    {
        private static Dictionary<uint, BKEvent<IMessage>> messages;

        /// <summary>
        /// BKFunc<opcode, IMessage>
        /// </summary>
        public static IMessageBuilder messageBuilder;

        public static void AddMessageListener(uint opcode, BKAction<IMessage> messageHanlder)
        {
            BKEvent<IMessage> item;
            if (!messages.TryGetValue(opcode, out item))
            {
                item = new BKEvent<IMessage>();
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

        public static IMessage BuildMessage(uint opcode)
        {
            if (messageBuilder != null)
            {
                return messageBuilder.BuildMessage(opcode);
            }

            return null;
        }

        public static IMessage BuildMessage(ByteArray steam)
        {
            if (messageBuilder != null)
            {
                return messageBuilder.BuildMessage(steam);
            }

            return null;
        }
    }
}
