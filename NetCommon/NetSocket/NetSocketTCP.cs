using System.Net.Sockets;

namespace GameCore.NetAlpha
{
    public class NetSocketTCP : NetSocket
    {
        public NetSocketTCP() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {

        }
    }
}
