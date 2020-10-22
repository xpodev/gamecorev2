using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ClientJoinRequestHandler : IClientJoinRequestHandler
    {
        public bool HandleClientJoinRequest(ClientInfo clientInfo, object joinRequest)
        {
            return true;
        }
    }
}
