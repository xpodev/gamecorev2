using GameCore.Multiplayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = File.ReadAllText("ServerConfiguration.json");
            ServerSettings settings = JsonConvert.DeserializeObject<ServerSettings>(json);

            Console.WriteLine($"[Application][Server] Binding on {settings.IpAddress}:{settings.Port}");
            JoinServer joinServer = new JoinServer(settings);

            joinServer.SetHandler(new ClientJoinRequestHandler());

            joinServer.OnClientJoin += (info) => Console.WriteLine($"Client Joined [{info.ClientInfo.ClientEndPoint}] Serial: {info.ClientInfo.Serial}");

            ClientManager manager = new ClientManager();
            
            Protocol.Instantiate(new DefaultClock());

            while (true)
            {
                joinServer.ReceiveClient(manager);
            }
        }
    }
}
