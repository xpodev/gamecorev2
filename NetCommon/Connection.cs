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

        protected Message<T> m_vTmpMessage;

        protected BlockingCollection<Message<T>> m_qOutgoingMessages = new BlockingCollection<Message<T>>();

        public BlockingCollection<Message<T>> OutgoingMessages
        {
            get
            {
                return m_qOutgoingMessages;
            }
        }

        public bool Connected
        {
            get
            {
                return m_rSocket.Connected;
            }
        }

        public Connection()
        {
            m_rSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public Connection(Socket socket)
        {
            System.Diagnostics.Debug.Assert(socket.ProtocolType == ProtocolType.Tcp);
            m_rSocket = socket;
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
                m_rSocket.Close();
            }
        }

        private void ReadHeader()
        {
            byte[] buffer = new byte[Marshal.SizeOf(m_vTmpMessage.Header)];
            SocketError socketError = default;
            m_rSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out socketError, (result) =>
            {
                int readBytesCount = m_rSocket.EndReceive(result);
                Console.WriteLine("Received " + readBytesCount + " Bytes");
                if (socketError == SocketError.Success)
                {
                    m_vTmpMessage = Message<T>.FromBytes(buffer);
                    if (m_vTmpMessage.Header.Size > 0)
                    {
                        ReadBody();
                    }
                    else
                    {
                        AddToIncomingMessages();
                    }
                }
                else
                {
                    Console.WriteLine($"ReadHeader Failed: ({socketError})");
                }
            }, this);
        }

        private void ReadBody()
        {
            byte[] buffer = new byte[m_vTmpMessage.Header.Size];
            SocketError socketError = default;
            m_rSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out socketError, (result) =>
            {
                int readBytesCount = m_rSocket.EndReceive(result);
                if (socketError == SocketError.Success)
                {
                    m_vTmpMessage.Insert(buffer);
                    AddToIncomingMessages();
                }
                else
                {
                    Console.WriteLine($"ReadBody Failed: ({socketError})");
                }
            }, this);
        }

        private void WriteMessage()
        {
            Message<T> msg = m_qOutgoingMessages.Take();
            byte[] bytes = msg.GetBytes();
            SocketError socketError = default;
            m_rSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, out socketError, (result) =>
            {
                int writeBytesCount = m_rSocket.EndSend(result);
                if (socketError == SocketError.Success)
                {
                    if (m_qOutgoingMessages.Count > 0)
                    {
                        WriteMessage();
                    }
                }
                else
                {
                    Console.WriteLine($"WriteMessage Failed: ({socketError})");
                }
            }, this);
        }

        protected internal virtual void AddToIncomingMessages()
        {
            ReadHeader();
        }
    }
}
