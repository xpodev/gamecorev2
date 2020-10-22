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
            // load server configuration
            string json = File.ReadAllText("ServerConfiguration.json");
            ServerSettings settings = JsonConvert.DeserializeObject<ServerSettings>(json);

            // create game server

            // open pipe, begin Local Database Thread (async Task)

            // begin send loop (async Task)

            // receive loop (Main Thread)
        }
    }
}
