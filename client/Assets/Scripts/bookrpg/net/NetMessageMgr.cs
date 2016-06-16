using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;

namespace bookrpg.net
{
    public static class NetMessageMgr
    {
        private static Dictionary<int, BKEvent<INetMessage>> messages;

        /// <summary>
        /// BKFunc<opcode, INetMessage>
        /// </summary>
        public static INetMessageBuilder messageBuilder;

        public static void AddMessageListener(int opcode, BKAction<INetMessage> messageHanlder)
        {
            BKEvent<INetMessage> item;
            if (!messages.TryGetValue(opcode, out item))
            {
                item = new BKEvent<INetMessage>();
            }
            item.AddListener(messageHanlder);
        }

        public static void RemoveMessageListener(int opcode, BKAction<INetMessage> messageHanlder)
        {
            BKEvent<INetMessage> item;
            if (messages.TryGetValue(opcode, out item))
            {
                item.RemoveListener(messageHanlder);
            }
        }

        public static void DispatchMessage(INetMessage message)
        {
            BKEvent<INetMessage> item;
            if (messages.TryGetValue(message.opcode, out item))
            {
                item.Invoke(message);
            }
        }

        public static void DispatchMessage(int opcode, INetMessage message)
        {
            BKEvent<INetMessage> item;
            if (messages.TryGetValue(opcode, out item))
            {
                item.Invoke(message);
            }
        }

        public static INetMessage BuildMessage(int opcode)
        {
            if (messageBuilder != null)
            {
                return messageBuilder.BuildMessage(opcode);
            }

            return null;
        }

        public static INetMessage BuildMessage(ByteArray steam)
        {
            if (messageBuilder != null)
            {
                return messageBuilder.BuildMessage(steam);
            }

            return null;
        }
    }
}
