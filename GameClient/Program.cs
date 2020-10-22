using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerType = GameClient.A;

namespace GameClient
{
    enum A
    {

    }

    class Program
    {
        public static void Main(string[] args)
        {
            GameClient<ServerType> client = new GameClient<ServerType>();
            while (true)
            {
                try
                {
                    client.ConnectToServer("localhost", 5060);
                    Console.WriteLine($"Successfully connected with serial: {client.Serial}");
                    client.Disconnect();
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
