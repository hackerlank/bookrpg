using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using bookrpg.core;
using bookrpg.mgr;

namespace bookrpg.net
{
    public class TcpRemoteServer : RemoteServer
    {
        private TcpClient client;

        private float heartbeatTime;

        public int hearbeatOpcode = 0;

        public bool autoReconnect;

        public bool useBigEndian;

        /// <summary>
        /// The heartbeat rate, e.g. 30s
        /// </summary>
        public int heartbeatRate;

        public TcpRemoteServer()
        {
            useBigEndian = !BitConverter.IsLittleEndian;

            client = new TcpClient();
            client.onConnect += onConnect;
            client.onReceive += onReceive;
            client.onError += onError;
        }

        public IEnumerator Connect(string host, int port)
        {
            client.Connect(host, port);
            while (!client.isConnected || client.error == null)
            {
                yield return null;
            }

            heartbeatTime = Time.time;
        }

        public IEnumerator Reconnect()
        {
            client.Reconnect();
            while (!client.isConnected || client.error == null)
            {
                yield return null;
            }

            heartbeatTime = Time.time;
        }

        public override bool SendMessage(INetMessage message)
        {
            if (client.isConnected)
            {
                return client.Send(message.Serialize()); 
            }

            return false;
        }

        public override void Close()
        {
            client.Dispose();
        }

        public override void Update()
        {
            if (heartbeatRate > 0 && Time.time - heartbeatTime >= heartbeatRate)
            {
                SendHeartbeat();
                heartbeatTime = Time.time;
            }
        }

        private void onConnect(TcpClient client)
        {
            Debug.LogFormat("Connected to {0}:{1}", client.host, client.port);
        }

        private void onReceive(TcpClient client, byte[] bytes)
        {
            using (var byteArray = new ByteArray(bytes))
            {
                byteArray.endian = useBigEndian ? Endian.BIG_ENDIAN : Endian.LITTLE_ENDIAN;
                var message = NetMessageMgr.BuildMessage(byteArray);

                if (message != null)
                {
                    NetMessageMgr.DispatchMessage(message);
                }
            }
        }

        private void onError(TcpClient client, string error)
        {
            //TODO tip
            Debug.LogError(error);
            if (autoReconnect)
            {
                client.Reconnect();
            }
        }

        private void SendHeartbeat()
        {
            var message = NetMessageMgr.BuildMessage(hearbeatOpcode);
            if (message != null)
            {
                SendMessage(message);
            }
        }

    }
}
