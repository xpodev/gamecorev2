using System.Net.Sockets;

namespace GameCore.NetAlpha
{
    public abstract class NetSocket : Socket
    {
        public NetSocket(AddressFamily family, SocketType socketType, ProtocolType protocol) : base(family, socketType, protocol) { }
    }
}
