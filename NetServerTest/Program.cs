using System;
using System.Threading;
using GameCore.Net;
using GameCore.Net.Server;

namespace NetTest
{
    enum CMD
    {
        Chat
    }

    class MyServer : ServerInterface<CMD>
    {
        public MyServer(ushort port) : base(port)
        {

        }

        protected override bool OnClientConnect(ServerConnection<CMD> connection)
        {
            Console.WriteLine($"[-----] Connect Request");
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
            MyServer server = new MyServer(3741);
            Thread acceptServerThread = new Thread(server.Start);
            acceptServerThread.Start();
            
            while (true)
            {
                // EATING CPU
                server.Update(uint.MaxValue);
            }
        }
    }
}
