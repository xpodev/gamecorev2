using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCore.Net.Server
{
    public class ServerInterface<T> where T : struct, Enum
    {
        private EndPoint m_rEndPoint;
        private Socket m_rSocket;

        private int m_nIDCounter = 10000;

        private Dictionary<int, ServerConnection<T>> m_qConnections = new Dictionary<int, ServerConnection<T>>();

        private BlockingCollection<OwnedMessage<T>> m_qIncomingMessages = new BlockingCollection<OwnedMessage<T>>();

        public ServerInterface(ushort port)
        {
            m_rEndPoint = new IPEndPoint(IPAddress.Any, port);
            m_rSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            m_rSocket.Bind(m_rEndPoint);
            m_rSocket.Listen();
        }

        ~ServerInterface()
        {
            Stop();
        }

        public void Start()
        {
            WaitForClientConnection();
        }

        public void Stop()
        {

        }

        public void MessageClient(ServerConnection<T> connection, Message<T> message)
        {
            if (connection is not null && connection.Connected)
            {
                connection.Send(message);
            } else
            {
                OnClientDisconnect(connection);
                // NullReferenceException may be thrown here
                m_qConnections.Remove(connection.UID);
            }
        }

        public void MessageAllClients(Message<T> message, ServerConnection<T> ignore = null)
        {

            List<int> invalidClients = new List<int>();

            foreach (ServerConnection<T> connection in m_qConnections.Values)
            {
                if (connection is not null && connection.Connected)
                {
                    if (connection != ignore)
                    {
                        connection.Send(message);
                    }
                } else
                {
                    OnClientConnect(connection);
                    invalidClients.Add(connection.UID);
                }
            }

            foreach (int uid in invalidClients)
            {
                m_qConnections.Remove(uid);
            }
        }

        public void Update(uint maxMessages = uint.MaxValue)
        {
            uint currentMessage = 0;
            CancellationToken cancellationToken = new CancellationToken(false);
            while (currentMessage < maxMessages)
            {
                OwnedMessage<T> message = m_qIncomingMessages.Take(cancellationToken);

                OnMessage(m_qConnections[message.UID], message.message);
                currentMessage++;

                if (m_qIncomingMessages.Count == 0)
                {
                    break;
                }
            }
        }

        protected virtual bool OnClientConnect(ServerConnection<T> connection)
        {
            return false;
        }

        protected virtual void OnClientDisconnect(ServerConnection<T> connection)
        {
            
        }

        protected virtual void OnMessage(ServerConnection<T> client, Message<T> message)
        {

        }

        private void WaitForClientConnection()
        {
            m_rSocket.BeginAccept((result) =>
            {
                Socket client = ((Socket)result.AsyncState).EndAccept(result);
                ServerConnection<T> connection = new ServerConnection<T>(m_qIncomingMessages, client);
                if (OnClientConnect(connection))
                {
                    if (connection.ConnectToClient(m_nIDCounter++))
                    {
                        m_qConnections.Add(connection.UID, connection);
                        connection.BeginListen();
                        Console.WriteLine($"[{connection.UID}] Connection Approved");
                    } else
                    {
                        Console.WriteLine($"[-----] Connection Denied");
                    }
                }

                WaitForClientConnection();
            }, m_rSocket);
        }
    }
}
