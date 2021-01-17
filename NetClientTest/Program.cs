using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GameCore.Net;
using GameCore.Net.Client;

namespace NetClientTest
{
    enum CMD
    {
        Chat
    }

    class MyClient : ClientInterface<CMD>
    {
        public void Process()
        {
            CancellationToken cancellationToken = new CancellationToken(false);
            while (true)
            {
                Message<CMD> message = IncomingMessages.Take(cancellationToken);
                Console.WriteLine(message.Header.Size);
                switch (message.Header.Id)
                {
                    case CMD.Chat:
                        Console.WriteLine("Chat: " + message.Extract<NetString<CMD>>());
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {message.Header.Id}");
                        break;
                }
            }
        }

        public void SendChat(NetString<CMD> msg)
        {
            Message<CMD> message = new Message<CMD>(CMD.Chat);
            message.Insert(msg);
            Send(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyClient client = new MyClient();

            if (client.Connect("localhost", 3741))
            {
                Console.WriteLine("Connected!");
            } else
            {
                Console.WriteLine("Connect Failed");
                return;
            }

            Thread procThread = new Thread(client.Process);
            procThread.Start();

            while (true)
            {
                client.SendChat(new NetString<CMD>(Console.ReadLine()));
            }
        }
    }
}
