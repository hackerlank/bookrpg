using System;
using System.IO;
using bookrpg.net;
using bookrpg.utils;

namespace bookrpg.net.protobuf
{
	public interface IProtobufMessage
	{
        void ParseFrom(ByteArray input);

        void WriteTo(ByteArray output);
	}
}
