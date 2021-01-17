using System;
using System.Collections.Concurrent;
using System.Net;

namespace GameCore.Net.Client
{
    public class ClientConnection<T> : Connection<T> where T : struct, Enum
    {
        protected BlockingCollection<Message<T>> m_qIncomingMessages = new BlockingCollection<Message<T>>();

        public BlockingCollection<Message<T>> IncomingMessages
        {
            get
            {
                return m_qIncomingMessages;
            }
        }

        public void ConnectToServer(EndPoint endPoint)
        {
            //m_rSocket.BeginConnect(endPoint, (result) =>
            //{
            //    m_rSocket.EndConnect(result);
            //    if (Connected)
            //    {
            //        OnConnectToServer();
            //    }
            //}, this);
            m_rSocket.Connect(endPoint);
            if (Connected)
            {
                OnConnectToServer();
            }
        }

        protected virtual void OnConnectToServer()
        {
            BeginListen();
        }

        protected override void AddToIncomingMessages()
        {
            m_qIncomingMessages.Add(m_vTmpMessage);
            base.AddToIncomingMessages();
        }
    }
}
