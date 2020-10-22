using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class GameServer<T> where T : Enum
    {
        public static GameServer<T> CurrentServer
        {
            get;
            private set;
        }

        public MessageSendQueue<T> SendQueue
        {
            get;
            private set;
        }

        public ClientManager ClientManager
        {
            get;
            private set;
        }

        public GameServer(GameServerSettings settings)
        {
            CurrentServer = this;
            SendQueue = new MessageSendQueue<T>(settings.OutputQueueSettings);
            ClientManager = new ClientManager();
        }
    }

    public struct GameServerSettings
    {
        [JsonProperty]
        public MessageSendQueueSettings OutputQueueSettings {
            get;
            private set;
        }


    }
}
