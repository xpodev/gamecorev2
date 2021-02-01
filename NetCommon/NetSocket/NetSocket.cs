using System.Net.Sockets;

namespace GameCore.Net
{
    public abstract class NetSocket : Socket
    {
        public NetSocket(AddressFamily family, SocketType socketType, ProtocolType protocol) : base(family, socketType, protocol) { }
    }
}
