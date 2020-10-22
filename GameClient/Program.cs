using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            GameClient client = new GameClient();
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
