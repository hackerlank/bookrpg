using UnityEngine;
using System;
using System.Collections;

namespace bookrpg.net
{
    public abstract class RemoteServer
    {
        private static RemoteServer mainServer;
        private static RemoteServer chatServer;

        public static RemoteServer main
        {
            get { return mainServer; }
        }

        public static RemoteServer chat
        {
            get { return chatServer; }
        }

        public static void Init(RemoteServer main, RemoteServer chat = null)
        {
            mainServer = main;
            chatServer = chat == null ? main : chat;
        }

        public long serverTime
        {
            get;
            protected set;
        }

        public virtual bool SendMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException(); 
        }

        public virtual void Update()
        {
            throw new NotImplementedException(); 
        } 
    }
}
