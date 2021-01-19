using System.Net.Sockets;

namespace GameCore.Net
{
    public abstract class NetSocket : Socket
    {
        public NetSocket(SocketType socketType, ProtocolType protocol) : base(socketType, protocol) { }
    }
}
