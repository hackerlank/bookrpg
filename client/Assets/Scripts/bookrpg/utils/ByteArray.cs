using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace bookrpg.net
{

    public enum Endian
    {
        LITTLE_ENDIAN  = 0,
        BIG_ENDIAN = 1,
    }

    public class ByteArray : IDisposable
    {
        private BinaryReader reader;
        private BinaryWriter writer;

        public MemoryStream stream { get; private set; }

        public ByteArray()
        {
            stream = new MemoryStream(1024);
            Init();
        }

        public ByteArray(int capacity)
        {
            stream = new MemoryStream(capacity);
            Init();
        }

        public ByteArray(byte[] bytes)
        {
            stream = new MemoryStream(bytes);
            Init();
        }

        public ByteArray(MemoryStream stream)
        {
            this.stream = stream;
            Init();
        }

        private void Init()
        {
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
            endian = BitConverter.IsLittleEndian ? Endian.LITTLE_ENDIAN : Endian.BIG_ENDIAN;
        }

        public Endian endian;

        public long length
        {
            get
            {
                return stream.Length;
            }
        }

        public long position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        public long  bytesAvailable
        {
            get
            {
                return stream.Length - stream.Position;
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public void Clear()
        {
            stream.SetLength(0);
            stream.Position = 0;
        }

        public void Close()
        {
            stream.Close();
        }

        public void Seek(int offset, SeekOrigin loc)
        {
            stream.Seek(offset, loc);
        }

        public int Read()
        {
            return reader.Read();
        }

        public int Read(char[] buffer, int index, int count)
        {
            return reader.Read(buffer, index, count);
        }

        public int Read(byte[] buffer, int index, int count)
        {
            return reader.Read(buffer, index, count);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        public short ReadInt16()
        {
            return reader.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            return reader.ReadUInt16();
        }

        public char ReadChar()
        {
            return reader.ReadChar();
        }

        public uint ReadUInt32()
        {
            return reader.ReadUInt32();
        }

        public int ReadInt32()
        {
            return reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return reader.ReadInt64();
        }

        public ulong ReadUInt64()
        {
            return reader.ReadUInt64();
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public decimal ReadDecimal()
        {
            return reader.ReadDecimal();
        }

        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        public sbyte ReadSByte()
        {
            return reader.ReadSByte();
        }

        public string ReadString()
        {
            return reader.ReadString();
        }

        public char[] ReadChars(int count)
        {
            return reader.ReadChars(count);
        }

        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        public void Write(byte value)
        {
            writer.Write(value);
        }

        public void Write(char value)
        {
            writer.Write(value);
        }

        public void Write(sbyte value)
        {
            writer.Write(value);
        }

        public void Write(short value)
        {
            writer.Write(value);
        }

        public void Write(ushort value)
        {
            writer.Write(value);
        }

        public void Write(int value)
        {
            writer.Write(value);
        }

        public void Write(uint value)
        {
            writer.Write(value);
        }

        public void Write(byte[] value)
        {
            writer.Write(value);
        }

        public void Write(byte[] value, int index, int count)
        {
            writer.Write(value, index, count);
        }

        public void Write(long value)
        {
            writer.Write(value);
        }

        public void Write(ulong value)
        {
            writer.Write(value);
        }

        public void Write(float value)
        {
            writer.Write(value);
        }

        public void Write(double value)
        {
            writer.Write(value);
        }

        public void Write(string value)
        {
            writer.Write(value);
        }

        public void Write(char[] value)
        {
            writer.Write(value);
        }

        public void Write(decimal value)
        {
            writer.Write(value);
        }

        public void Write(char[] value, int index, int count)
        {
            writer.Write(value, index, count);
        }
    }
}
