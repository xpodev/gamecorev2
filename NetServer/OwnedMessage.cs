using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net.Server
{
    internal struct OwnedMessage<T> where T : struct, Enum
    {
        public Message<T> message;

        public int UID;
    }
}
