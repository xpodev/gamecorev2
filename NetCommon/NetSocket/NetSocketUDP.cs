using System.Net.Sockets;

namespace GameCore.NetAlpha
{
    public class NetSocketUDP : NetSocket
    {
        public NetSocketUDP() : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {

        }
    }
}
