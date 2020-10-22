using GameCore.Serialization;
using System;
using System.Net;


namespace GameCore.Multiplayer
{
    [Serializable]
    [SerializeBinary]
    public class ProtocolSettings
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

        public Type[] RegisteredTypes
        {
            get;
            private set;
        }

        public ProtocolSettings(IPEndPoint endPoint)
        {
            GameServerEndPoint = endPoint;
            Version = Protocol.Version;
            RegisteredTypes = Protocol.CurrentProtocol.GetResiteredTypes();
        }
    }
}
