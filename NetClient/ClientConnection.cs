using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace GameCore.NetAlpha.Client
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

        public ClientConnection(NetSocket socket) : base(socket)
        {

        }

        public void ConnectToServer(EndPoint endPoint)
        {
            IPEndPoint ipEndPoint = (IPEndPoint)endPoint;
            try
            {
                if (ipEndPoint.Address.ScopeId == 0)
                {
                    return;
                }
            } catch (SocketException)
            {
                
            }
            m_rSocket.Connect(ipEndPoint);
            if (Connected)
            {
                m_rRemoteEndPoint = ipEndPoint;
                if (m_rSocket.ProtocolType == ProtocolType.Udp)
                {
                    Send(new Message<T>());
                    WriteMessage();
                }
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
