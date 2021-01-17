﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net.Client
{
    public class ClientInterface<T> where T : struct, Enum
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
                if (m_rConnection is not null)
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

                m_rConnection = new ClientConnection<T>();

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
