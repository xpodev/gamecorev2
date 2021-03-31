using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCore.NetAlpha.Server
{
    public class ServerInterface<T, SocketType> where T : struct, Enum where SocketType : NetSocket, new()
    {
        private EndPoint m_rEndPoint;
        private Socket m_rSocket;

        private ulong m_nCurrentUID = 10000;

        protected ulong CurrentUID
        {
            get
            {
                return m_nCurrentUID;
            }
            set
            {
                m_nCurrentUID = value;
            }
        }

        public EndPoint LocalEndPoint
        {
            get
            {
                return m_rEndPoint;
            }
        }

        private ServerConnection<T> m_rUDPConnection;

        private Dictionary<ulong, ServerConnection<T>> m_qConnections = new Dictionary<ulong, ServerConnection<T>>();

        private BlockingCollection<OwnedMessage<T>> m_qIncomingMessages = new BlockingCollection<OwnedMessage<T>>();

        public ServerInterface(EndPoint mainServerLocalEndPoint, int backlog = 10)
        {
            m_rEndPoint = mainServerLocalEndPoint;
            m_rSocket = new SocketType();
            m_rSocket.Bind(m_rEndPoint);
            if (m_rSocket.ProtocolType == ProtocolType.Tcp)
            {
                m_rSocket.Listen(backlog);
            } else
            {
                m_rUDPConnection = new ServerConnection<T>(m_qIncomingMessages, m_rSocket);
            }
        }

        ~ServerInterface()
        {
            Stop();
        }

        public void Start()
        {
            if (m_rSocket.ProtocolType == ProtocolType.Udp)
            {
                m_rUDPConnection.BeginListen();
            } else
            {
                WaitForClientConnection();
            }
        }

        public void Stop()
        {
            m_rSocket.Close();
        }

        public void MessageClient(ServerConnection<T> connection, Message<T> message)
        {
#if NET5_0
            if (connection is not null && connection.Connected)
#else
            if (connection != null && connection.Connected)
#endif
            {
                connection.Send(message);
            }
            else if (m_rSocket.ProtocolType == ProtocolType.Tcp)
            {
                OnClientDisconnect(connection);
                // NullReferenceException may be thrown here
                m_qConnections.Remove(connection.UID);
            }
        }

        public void MessageAllClients(Message<T> message, ServerConnection<T> ignore = null)
        {

            List<ulong> invalidClients = new List<ulong>();
            if (m_rSocket.ProtocolType == ProtocolType.Tcp)
            {
                foreach (ServerConnection<T> connection in m_qConnections.Values)
                {
#if NET5_0
                    if (connection is not null && connection.Connected)
#else
                    if (connection != null && connection.Connected)
#endif
                    {
                        if (connection != ignore)
                        {
                            connection.Send(message);
                        }
                    }
                    else
                    {
                        OnClientDisconnect(connection);
                        invalidClients.Add(connection.UID);
                    }
                }

                foreach (ulong uid in invalidClients)
                {
                    m_qConnections.Remove(uid);
                }
            } else
            {
                foreach (ServerConnection<T> connection in m_qConnections.Values)
                {
#if NET5_0
                    if (connection is not null)
#else
                    if (connection != null)
#endif
                    {
                        if (connection != ignore)
                        {
                            connection.Send(message);
                        }
                    }
                }
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

        protected virtual ulong GetNextUID(ulong oldUID)
        {
            return oldUID + 1;
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
                Socket client = ((SocketType)result.AsyncState).EndAccept(result);
                ServerConnection<T> connection = new ServerConnection<T>(m_qIncomingMessages, client);
                if (OnClientConnect(connection))
                {
                    if (connection.ConnectToClient((m_nCurrentUID = GetNextUID(m_nCurrentUID))))
                    {
                        m_qConnections.Add(connection.UID, connection);
                        connection.BeginListen();
                        Console.WriteLine($"[{connection.UID}] Connection Approved");
                    }
                    else
                    {
                        Console.WriteLine($"[-----] Connection Denied");
                    }
                }

                WaitForClientConnection();
            }, m_rSocket);
        }

        public void AddClient(ServerConnection<T> connection, bool nextUID = false)
        {
            m_qConnections.Add(connection.UID, connection);
            if (nextUID)
            {
                m_nCurrentUID = GetNextUID(m_nCurrentUID);
            }
        }

        public ServerConnection<T> CreateConnection(Socket socket, bool autoAdd = true, bool autoUID = false, ulong uid = 0)
        {
            ServerConnection<T> serverConnection = new ServerConnection<T>(m_qIncomingMessages, socket);

            if (serverConnection.ConnectToClient(autoUID ? m_nCurrentUID : uid, socket.ProtocolType == ProtocolType.Udp))
            {
                if (autoAdd)
                {
                    AddClient(serverConnection, autoUID);
                }
                return serverConnection;
            }
            return null;
        }

        public void RemoveClient(ulong uid)
        {
            m_qConnections.Remove(uid);
        }
    }
}
