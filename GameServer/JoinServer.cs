using GameCore.Multiplayer;
using GameCore.Serialization;
using System.Net;
using System.Net.Sockets;


namespace GameServer
{
    public class JoinServer
    {
        private readonly TcpListener tcp;

        public delegate void OnClientJoinCallback(ClientHandler clientHandler);

        public event OnClientJoinCallback OnClientJoin;

        public IClientJoinRequestHandler ClientJoinRequestHandler
        {
            get;
            private set;
        }

        public ServerInfo Info
        {
            get;
            private set;
        }

        public JoinServer(ServerSettings settings)
        {
            Info = new ServerInfo(settings.ServerName);
            tcp = new TcpListener(IPAddress.Parse(settings.IpAddress), settings.Port);
            tcp.Server.SendBufferSize = settings.OutBufferSize;
            tcp.Server.ReceiveBufferSize = settings.InBufferSize;
            tcp.Start(10);
        }

        public void SetHandler(IClientJoinRequestHandler handler)
        {
            ClientJoinRequestHandler = handler;
        }

        public void ReceiveClient(ClientManager clientManager)
        {
            if (ClientJoinRequestHandler is null)
            {
                throw new System.Exception("Handler not set");
            }
            TcpClient client = tcp.AcceptTcpClient();
            BinarySerializer serializer = new BinarySerializer();

            byte[] readBuffer = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(readBuffer, 0, readBuffer.Length);
            object requestInfo = serializer.Deserialize(readBuffer);

            IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            ClientInfo clientInfo = new ClientInfo(clientEndPoint.Address, clientEndPoint.Port);

            ProtocolSettings protocolSettings = new ProtocolSettings((IPEndPoint)client.Client.LocalEndPoint);

            if (ClientJoinRequestHandler.HandleClientJoinRequest(clientInfo, requestInfo))
            {
                ClientHandler handler = clientManager.AddClient(clientInfo);
                if (OnClientJoin != null)
                {
                    OnClientJoin.Invoke(handler);
                }
                handler.RunClient();
            }

            Message msg = Protocol.CurrentProtocol.CreateCustomMessage(protocolSettings, clientInfo.Serial);
            byte[] data = serializer.Serialize(msg);
            client.GetStream().Write(data, 0, data.Length);

            client.Close();
        }
    }
}
