using GameCore.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class ClientHandler
    {
        private readonly ClientSocket client;

        public ClientInfo ClientInfo
        {
            get;
            private set;
        }

        public ClientHandler(ClientInfo info)
        {
            ClientInfo = info;
            client = new ClientSocket(this);
        }

        public void Send<T>(T obj)
        {
            client.BeginSendObject(obj, ClientInfo.Serial);
        }

        public void OnReceiveMessage(Message msg)
        {
            if (msg.Serial != ClientInfo.Serial)
            {
                return;
            }
            Protocol.CurrentProtocol.HandleMessage(msg);
        }

        public void RunClient()
        {
            Thread thread = new Thread(client.ClientLoop)
            {
                Name = $"Client: {ClientInfo.Serial}"
            };
            client.Running = true;
            thread.Start();
        }
    }
}
