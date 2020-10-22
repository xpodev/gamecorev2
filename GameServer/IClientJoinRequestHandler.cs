using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IClientJoinRequestHandler
    {
        bool HandleClientJoinRequest(ClientInfo clientInfo, object joinRequest);
    }
}
