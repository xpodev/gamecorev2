using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GameCore.Multiplayer;
using GameCore.Serialization;


namespace GameServer
{
    class ClientSocket : Socket
    {
        private class ReceiveData
        {
            public byte[] buffer;
            public bool isReceiving;
        }

        private ClientHandler Handler
        {
            get;
            set;
        }

        public const int DefaultBufferSize = 4096;

        public const int DefaultSendTimeout = 1;

        public bool Running
        {
            get;
            set;
        }

        public ClientSocket(ClientHandler handler, int bufferSize = DefaultBufferSize, int sendTimeout = DefaultBufferSize) : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Handler = handler;
            if (bufferSize <= 0)
            {
                throw new ArgumentException($"{bufferSize} is not a valid buffer size");
            }
            SendBufferSize = ReceiveBufferSize = bufferSize;
            SendTimeout = sendTimeout;
        }

        public void BeginSendObject<T>(T obj, long clientSerial)
        {
            BinarySerializer serializer = new BinarySerializer();
            byte[] data = serializer.Serialize(Protocol.CurrentProtocol.CreateMessage(obj, clientSerial));
            // TODO: use socket error code
            BeginSend(data, 0, data.Length, SocketFlags.None, EndSendCallback, null);
        }

        private void EndSendCallback(IAsyncResult result)
        {
            
        }

        private void EndReceiveCallback(IAsyncResult result)
        {
            ReceiveData receiveData = (ReceiveData)result.AsyncState;
            byte[] data = receiveData.buffer;
            if (data.Length == 0)
            {
                return;
            }
            BinarySerializer serializer = new BinarySerializer();
            Message msg = serializer.Deserialize<Message>(data);
            receiveData.isReceiving = false;
            Handler.OnReceiveMessage(msg);
        }

        public void ClientLoop()
        {
            try
            {
                ReceiveData receiveData = new ReceiveData();
                while (Running)
                {
                    if (receiveData.isReceiving)
                    {
                        continue;
                    }
                    receiveData.buffer = new byte[ReceiveBufferSize];
                    BeginReceive(receiveData.buffer, 0, receiveData.buffer.Length, SocketFlags.None, EndReceiveCallback, receiveData);
                    receiveData.isReceiving = true;
                }
            } catch (SocketException)
            {
                ClientManager.CurrentManager.RemoveClient(Handler.ClientInfo);
                Console.WriteLine($"Client disconnected. Serial: {Handler.ClientInfo.Serial}");
            }
        }
    }
}
