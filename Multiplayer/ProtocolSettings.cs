using GameCore.Serialization;
using System;
using System.Net;


namespace GameCore.Multiplayer
{
    [Serializable]
    [SerializeBinary]
    public class ProtocolSettings<T> where T : Enum
    {
        public Version Version
        {
            get;
            private set;
        }

        //public int ReceiveBufferSize, SendBufferSize;

        public IPEndPoint GameServerEndPoint
        {
            get;
            private set;
        }

        public Type EnumType
        {
            get;
            private set;
        }

        public ProtocolSettings(IPEndPoint endPoint)
        {
            GameServerEndPoint = endPoint;
            Version = Protocol<T>.Version;
            EnumType = typeof(T);
        }
    }
}
