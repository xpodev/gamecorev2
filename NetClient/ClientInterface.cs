using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net.Client
{
    public class ClientInterface<T, SocketType> where T : struct, Enum where SocketType : NetSocket, new()
    {
        private ClientConnection<T> m_rConnection;

        public BlockingCollection<Message<T>> IncomingMessages
        {
            get
            {
                return m_rConnection.IncomingMessages;
            }
        }

        public BlockingCollection<Message<T>> OutgoingMessages
        {
            get
            {
                return m_rConnection.OutgoingMessages;
            }
        }

        public bool Connected
        {
            get
            {
#if NET5_0
                if (m_rConnection is not null)
#else
                if (m_rConnection != null)
#endif
                {
                    return m_rConnection.Connected;
                }
                return false;
            }
        }

        ~ClientInterface()
        {
            Disconnect();
        }

        public bool Connect(string host, ushort port)
        {
            try
            {
                IPAddress[] ips = Dns.GetHostEntry(host).AddressList;

                m_rConnection = new ClientConnection<T>(new SocketType());

                for (int i = 0; i < ips.Length; ++i)
                {
                    try
                    {
                        m_rConnection.ConnectToServer(new IPEndPoint(ips[i], port));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return Connected;
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public void Disconnect()
        {
            if (Connected)
            {
                m_rConnection.Disconnect();
            }
        }

        public bool Send(Message<T> message)
        {
            if (Connected)
            {
                m_rConnection.Send(message);
                return true;
            }
            return false;
        }
    }
}
