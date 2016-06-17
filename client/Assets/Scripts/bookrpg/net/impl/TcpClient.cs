using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using bookrpg.core;
using bookrpg.utils;

namespace bookrpg.net
{
    /// <summary>
    /// Packet based async tcp client.
    /// </summary>
    public class TcpClient : IDispose
    {
        public Socket socket { get; private set; }

        public string error { get; private set; }

        public BKEvent<TcpClient> onConnect = new BKEvent<TcpClient>();

        public BKEvent<TcpClient, byte[]> onReceive = new BKEvent<TcpClient, byte[]>();

        public BKEvent<TcpClient, string> onError = new BKEvent<TcpClient, string>();

        public object custumData;

        public string host { get; private set; }

        public int port { get; private set; }

        //128KB
        private int bufferSize = 131072;

        private byte[] curSendBytes;

        private int curSendBytesIndex;

        private SocketAsyncEventArgs sendSAEA;

        private SocketAsyncEventArgs receiveSAEA;

        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private ByteArray receiveStream = new ByteArray();

        private bool isSending;

        private int packetLength = 0;

        private const int PACKET_LENGTH_SIZE = 4;

        private ByteArray packetLenArray = new ByteArray(PACKET_LENGTH_SIZE);


        public TcpClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sendSAEA = new SocketAsyncEventArgs();
            sendSAEA.SetBuffer(new byte[bufferSize], 0, bufferSize);
            sendSAEA.Completed += OnSocketCompleted;

            receiveSAEA = new SocketAsyncEventArgs();
            receiveSAEA.SetBuffer(new byte[bufferSize], 0, bufferSize);
            receiveSAEA.Completed += OnSocketCompleted;
        }

        public void Dispose()
        {
            if (this.socket != null)
            {
                this.socket.Close();
                sendQueue.Clear();
                onReceive.RemoveAllListeners();
                onError.RemoveAllListeners();
                receiveStream.Close();
                packetLenArray.Close();
            }
            this.socket = null;
        }

        public bool hasDisposed
        {
            get{ return socket == null; }
        }

        public bool isConnected
        {
            get
            {
                return socket != null && socket.Connected;
            }
        }

        public void Connect(string host, int port)
        {
            if (hasDisposed)
            {
                error = "instance has disposed";
                onError.Invoke(this, error);
                return;
            }

            this.host = host;
            this.port = port;

            IPAddress ipAddr;
            if (IPAddress.TryParse(host, out ipAddr))
            {
                sendSAEA.RemoteEndPoint = new IPEndPoint(ipAddr, port);
                Reconnect();
            } else
            {
                Dns.BeginGetHostAddresses(host, (result) =>
                {
                    var addr = Dns.EndGetHostAddresses(result);
                    if (addr.Length > 0)
                    {
                        sendSAEA.RemoteEndPoint = new IPEndPoint(addr[0], port);
                        Reconnect();
                    } else
                    {
                        error = "TcpClient.Connect, unknown host: " + host;
                        onError.Invoke(this, error);
                    }
                }, null);
            }
        }

        public void Reconnect()
        {
            if (hasDisposed || socket.Connected)
            {

                if (hasDisposed)
                {
                    error = "instance has disposed";
                    onError.Invoke(this, error);
                } else
                {
                    onConnect.Invoke(this);
                }

                return;
            }

            if (!socket.ConnectAsync(sendSAEA))
            {
                OnSocketCompleted(socket, sendSAEA);
            }
        }

        public void Disconnect()
        {
            if (hasDisposed)
            {
                return;
            }
            socket.Disconnect(true);
        }

        public bool Send(byte[] bytes)
        {
            if (hasDisposed || !socket.Connected)
            {
                error = hasDisposed ? "instance has disposed" : "not connected";
                onError.Invoke(this, error);
                return false;
            }

            sendQueue.Enqueue(bytes);

            if (!isSending)
            {
                DoSend();
            } 

            return true;
        }

        private void Receive()
        {
            if (!socket.ReceiveAsync(receiveSAEA))
            {
                DoReceive();
            }
        }

        private void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
        {
            error = null;

            if (e.SocketError != SocketError.Success)
            {
                error = e.SocketError.ToString();
                onError.Invoke(this, error);
                return;
            }

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0)
                {
                    DoReceive();
                } else
                {
                    error = "Server disconnet initiative";
                    onError.Invoke(this, error);
                }
            } else if (e.LastOperation == SocketAsyncOperation.Send)
            {
                isSending = false;
                DoSend();
                
            } else if (e.LastOperation == SocketAsyncOperation.Connect)
            {
                onConnect.Invoke(this);
                Receive();
            }
        }

        private bool DoSend()
        {
            if (sendQueue.Count == 0 && curSendBytes == null)
            {
                isSending = false;
                return false;
            }

            var bytes = curSendBytes != null ? curSendBytes : sendQueue.Dequeue();

            int count = bytes.Length;

            if (count + PACKET_LENGTH_SIZE - curSendBytesIndex > bufferSize)
            {
                count = bufferSize - PACKET_LENGTH_SIZE;
                curSendBytesIndex += count;
                curSendBytes = bytes;

            } else
            {
                curSendBytes = null;
                curSendBytesIndex = 0;
            }

            if (curSendBytesIndex == 0)
            {
                var lengthBytes = BitConverter.GetBytes(bytes.Length);
                Buffer.BlockCopy(lengthBytes, 0, sendSAEA.Buffer, 0, PACKET_LENGTH_SIZE);
                Buffer.BlockCopy(bytes, 0, sendSAEA.Buffer, PACKET_LENGTH_SIZE, count);
            } else
            {
                Buffer.BlockCopy(bytes, curSendBytesIndex, sendSAEA.Buffer, 0, count);
            }
            sendSAEA.SetBuffer(0, count);
            socket.SendAsync(sendSAEA);

            isSending = true;
            return true;
        }

        private void DoReceive()
        {
            receiveStream.Write(receiveSAEA.Buffer, 0, receiveSAEA.BytesTransferred);
            ReadDataPacket(receiveStream);

            if (receiveStream.bytesAvailable <= 0)
            {
                receiveStream.Clear();
            }
//            var recvBytes = new byte[receiveSAEA.BytesTransferred];
//            Buffer.BlockCopy(receiveSAEA.Buffer, 0, recvBytes, 0, receiveSAEA.BytesTransferred);
//            var recvStr = Encoding.ASCII.GetString(recvBytes, 0, receiveSAEA.BytesTransferred);
//            Debug.Log(recvStr);
           
            Receive();
        }

        /**
         * 从data中分离出数据包
         * @param data
         */
        private void ReadDataPacket(ByteArray data)
        {
            while (data.bytesAvailable > 0)
            {
                //开始读取一个新包
                if (packetLength == 0)
                {
                    if (data.bytesAvailable > PACKET_LENGTH_SIZE)
                    {
                        packetLength = data.ReadInt32();
                    } else
                    {
                        break;
                    }
                }

                if (data.bytesAvailable >= packetLength)
                {
                    //读取完整包
                    var packet = data.ReadBytes(packetLength);
                    onReceive.Invoke(this, packet);
                    packetLength = 0;
                } else
                {
                    //等待后续数据
                    break;
                }
            }
        }

        private byte[] WritePacketLength(int length)
        {
            packetLenArray.position = 0;
            packetLenArray.Write(length);
            return packetLenArray.ToArray();
        }
    }
}
