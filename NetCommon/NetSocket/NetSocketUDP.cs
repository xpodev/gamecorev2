using System.Net.Sockets;

namespace GameCore.Net
{
    public class NetSocketUDP : NetSocket
    {
        public NetSocketUDP() : base(SocketType.Dgram, ProtocolType.Udp)
        {

        }
    }
}
