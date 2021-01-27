using System.Net.Sockets;

namespace GameCore.Net
{
    public class NetSocketTCP : NetSocket
    {
        public NetSocketTCP() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {

        }
    }
}
