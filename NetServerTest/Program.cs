using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using GameCore.Net;
using GameCore.Net.Server;

namespace NetTest
{
    enum JoinServerCommands
    {
        JoinServer
    }

    enum CMD
    {
        Chat
    }

    class JoinServer<T> : ServerInterface<T, NetSocketTCP> where T : struct, Enum
    {
        List<ServerConnection<T>> m_rServers = new List<ServerConnection<T>>();

        public JoinServer(ushort port) : base(new IPEndPoint(IPAddress.Any, port))
        {
            
        }

        public bool AddGameServer<GameServerSocketType>(ServerInterface<T, GameServerSocketType> server) where GameServerSocketType : NetSocket, new()
        {
            NetSocket socket = new GameServerSocketType();
            socket.Connect(server.LocalEndPoint);
            ServerConnection<T> connection = server.CreateConnection(socket, true, true);
            if (connection is not null)
            {
                m_rServers.Add(connection);
                return true;
            }
            return false;
        }

        protected override bool OnClientConnect(ServerConnection<T> connection)
        {
            return true;
        }

        protected override void OnMessage(ServerConnection<T> client, Message<T> message)
        {
            
        }

        private void Join(ServerConnection<T> client, int serverIndex)
        {

        }
    }

    class MyServer : ServerInterface<CMD, NetSocketUDP>
    {
        public MyServer(ushort port) : base(new IPEndPoint(IPAddress.Any, port))
        {

        }

        protected override bool OnClientConnect(ServerConnection<CMD> connection)
        {
            Console.WriteLine($"[-----] Connect Request");
            return true;
        }

        protected override bool OnClientConnect(ServerConnection<CMD> connection, Message<CMD> connectionMessage)
        {
            return true;
        }

        protected override void OnClientDisconnect(ServerConnection<CMD> connection)
        {
            Console.WriteLine($"[{connection.UID}] Disconnected");
        }

        protected override void OnMessage(ServerConnection<CMD> client, Message<CMD> msg)
        {
            switch (msg.Header.Id)
            {
                case CMD.Chat:
                    MessageAllClients(msg, client);
                    break;
                default:
                    Console.WriteLine($"Unknown command: {msg.Header.Id}");
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyServer server = new MyServer(3740, 3741);
            Thread acceptServerThread = new Thread(server.Start);
            acceptServerThread.Start();
            
            while (true)
            {
                server.Update(uint.MaxValue);
            }
        }
    }
}
