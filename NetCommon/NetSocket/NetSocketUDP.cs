using System.Net.Sockets;

namespace GameCore.Net
{
    public class NetSocketUDP : NetSocket
    {
        public NetSocketUDP() : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {

        }
    }
}
