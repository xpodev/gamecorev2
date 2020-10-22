using GameCore.Multiplayer;
using GameCore.Serialization;
using System;
using System.Net.Sockets;


namespace GameClient
{
    public class GameClient
    {
        private UdpClient udp;

        public long Serial
        {
            get;
            private set;
        }

        public GameClient()
        {
            Protocol.Instantiate(new DefaultClock());
        }

        public void ConnectToServer(string host, short port)
        {
            TcpClient client = new TcpClient(host, port);
            client.GetStream().Write(new byte[] { 0 }, 0, 1);
            // TODO: this 4096 should not be hardcoded
            byte[] buffer = new byte[4096];
            client.GetStream().Read(buffer, 0, buffer.Length);
            BinarySerializer serializer = new BinarySerializer();
            Message msg = serializer.Deserialize<Message>(buffer);

            ProtocolSettings protocolSettings = (ProtocolSettings)msg.Object;

            if (!Protocol.CurrentProtocol.ValidateProtocolSettings(protocolSettings))
            {
                throw new Exception("Protocol settings mismatch");
            }

            Serial = msg.Serial;
            if (Serial == 0)
            {
                throw new Exception("Could not join server (invalid serial 0)");
            }

            udp = new UdpClient(host, port);
        }

        public void Disconnect()
        {
            udp.Close();
            Serial = 0;
        }

        public void Send<T>(T obj)
        {
            Message msg =Protocol.CurrentProtocol.CreateMessage(obj, Serial);
            BinarySerializer serializer = new BinarySerializer();
            byte[] data = serializer.Serialize(msg);
            udp.Send(data, data.Length);
        }
    }
}
