using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace GameCore.Net
{
    public class Connection<T> where T : struct, Enum
    {
        protected Socket m_rSocket;

        protected BlockingCollection<Message<T>> m_qOutgoingMessages = new BlockingCollection<Message<T>>();

        protected EndPoint m_rRemoteEndPoint = new IPEndPoint(0, 0);

        public BlockingCollection<Message<T>> OutgoingMessages
        {
            get
            {
                return m_qOutgoingMessages;
            }
        }

        protected Message<T> m_vTmpMessage;

        public bool Connected
        {
            get
            {
                return m_rSocket.Connected;
            }
        }

        public EndPoint EndPoint
        {
            get
            {
                return m_rRemoteEndPoint;
            }
            set
            {
                m_rRemoteEndPoint = value;
            }
        }

        public Connection(Socket socket)
        {
            m_rSocket = socket;
        }

        public void Close()
        {
            m_rSocket.Close();
        }

        public void BeginListen()
        {
            ReadHeader();
        }

        public void Send(Message<T> message)
        {
            bool bWritingMessages = m_qOutgoingMessages.Count > 0;
            m_qOutgoingMessages.Add(message);
            if (!bWritingMessages)
            {
                WriteMessage();
            }
        }

        public void Disconnect()
        {
            if (Connected)
            {
                Close();
            }
        }

        protected void ReadHeader()
        {
            byte[] buffer = new byte[Marshal.SizeOf(m_vTmpMessage.Header)];
            try
            {
                m_rSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.Partial, ref m_rRemoteEndPoint, (result) =>
                {
                    try
                    {
                        int readBytesCount = m_rSocket.EndReceiveFrom(result, ref m_rRemoteEndPoint);
                        m_vTmpMessage = Message<T>.FromBytes(buffer);
                        if (m_vTmpMessage.Header.Size > 0)
                        {
                            ReadBody();
                        }
                        else
                        {
                            AddToIncomingMessages();
                        }
                    } catch (SocketException)
                    {
                        Close();
                    }
                }, this);

            } catch (SocketException)
            {
                Close();
            }
        }

        protected void ReadBody()
        {
            byte[] buffer = new byte[m_vTmpMessage.Header.Size];
            try
            {
                m_rSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref m_rRemoteEndPoint, (result) =>
                {
                    try
                    {
                        int readBytesCount = m_rSocket.EndReceive(result);
                        m_vTmpMessage.Insert(buffer);
                        AddToIncomingMessages();
                    } catch (SocketException)
                    {
                        Close();
                    }
                }, this);
            } catch (SocketException)
            {
                Close();
            }
        }

        protected void WriteMessage()
        {
            m_vTmpMessage = m_qOutgoingMessages.Take();
            byte[] bytes = m_vTmpMessage.GetBytes();
            int size = Marshal.SizeOf(m_vTmpMessage.Header);
            try
            {
                m_rSocket.BeginSendTo(bytes, 0, size, SocketFlags.None, m_rRemoteEndPoint, (result) =>
                {
                    try
                    {
                        int writeHeaderCount = m_rSocket.EndSendTo(result);
                        if (m_vTmpMessage.Header.Size > 0)
                        {
                            m_rSocket.BeginSendTo(bytes, size, m_vTmpMessage.Length, SocketFlags.None, m_rRemoteEndPoint, (result) =>
                            {
                                try
                                {
                                    int writeBodyCount = m_rSocket.EndSendTo(result);
                                    if (m_qOutgoingMessages.Count > 0)
                                    {
                                        WriteMessage();
                                    }
                                } catch (SocketException)
                                {
                                    Close();
                                }
                            }, this);
                        }
                    } catch (SocketException e)
                    {
                        Close();
                    }
                }, this);
            } catch (SocketException)
            {
                Close();
            }
        }

        protected virtual void AddToIncomingMessages()
        {
            ReadHeader();
        }
    }
}
