using System;
using System.IO;
using bookrpg.net;

namespace bookrpg.net.protobuf
{
	public interface IMessage
	{
        void ParseFrom(ByteArray input);

        void WriteTo(ByteArray output);
	}
}
