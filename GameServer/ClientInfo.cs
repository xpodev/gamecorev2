using System;
using System.Collections.Generic;
using System.Net;

namespace GameServer
{
    public class ClientInfo
    {
        public IPEndPoint ClientEndPoint
        {
            get;
            private set;
        }

        public long Serial
        {
            get;
            private set;
        }

        public ClientInfo(IPAddress address, int port)
        {
            ClientEndPoint = new IPEndPoint(address, port);
            Serial = default;
        }

        public bool IsValid()
        {
            return !(Serial == 0);
        }

        public void SetSerial(long newSerial)
        {
            Serial = newSerial;
        }
    }
}
