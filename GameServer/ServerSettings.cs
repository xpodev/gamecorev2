using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace GameServer
{
    public class ServerSettings
    {
        [JsonProperty]
        public string ServerName
        {
            get;
            private set;
        }

        [JsonProperty]
        public string IpAddress
        {
            get;
            private set;
        }

        [JsonProperty]
        public short Port
        {
            get;
            private set;
        }

        [JsonProperty]
        public int InBufferSize
        {
            get;
            private set;
        }

        [JsonProperty]
        public int OutBufferSize
        {
            get;
            private set;
        }
    }
}
