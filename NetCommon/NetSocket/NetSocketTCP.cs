using System.Net.Sockets;

namespace GameCore.Net
{
    public class NetSocketTCP : NetSocket
    {
        public NetSocketTCP() : base(SocketType.Stream, ProtocolType.Tcp)
        {

        }
    }
}
