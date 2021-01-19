using System;

namespace GameCore.Net.Server
{
    internal struct OwnedMessage<T> where T : struct, Enum
    {
        public Message<T> message;

        public ulong UID;
    }
}
