using GameCore.Multiplayer;
using GameCore.Serialization;
using System;
using System.Net.Sockets;



namespace GameClient
{
    public class GameClient<T> where T : Enum
    {
        private UdpClient udp;

        public long Serial
        {
            get;
            private set;
        }

        public GameClient()
        {
            Protocol<T>.Instantiate(new DefaultClock());
        }

        public void ConnectToServer(string host, short port)
        {
            TcpClient client = new TcpClient(host, port);
            client.GetStream().Write(new byte[] { 0 }, 0, 1);
            // TODO: this 4096 should not be hardcoded
            byte[] buffer = new byte[4096];
            client.GetStream().Read(buffer, 0, buffer.Length);
            BinarySerializer serializer = new BinarySerializer();
            Message<T> msg = serializer.Deserialize<Message<T>>(buffer);

            ProtocolSettings<T> protocolSettings = (ProtocolSettings<T>)msg.Object;

            if (!Protocol<T>.CurrentProtocol.ValidateProtocolSettings(protocolSettings))
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

        public void Send<TObject>(T command, TObject obj)
        {
            Message<T> msg = Protocol<T>.CurrentProtocol.CreateMessage(command, obj, Serial);
            BinarySerializer serializer = new BinarySerializer();
            byte[] data = serializer.Serialize(msg);
            udp.Send(data, data.Length);
        }
    }
}
