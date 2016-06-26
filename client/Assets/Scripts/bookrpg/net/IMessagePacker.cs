using UnityEngine;
using System.Collections;
using bookrpg.utils;

namespace bookrpg.net
{
    public interface IMessagePacker
    {
        void addMessageParser<T>(uint opcode) where T : IMessage;

        IMessage CreateMessage(uint opcode);

        byte[] PackMessage(IMessage message, bool useBigEndian = false);

        IMessage UnpackMessage(byte[] bytes, bool useBigEndian = false);
    }
}
