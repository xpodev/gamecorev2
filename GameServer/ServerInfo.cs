using GameCore.Serialization;
using System;


namespace GameServer
{
    [Serializable]
    [SerializeBinary]
    public struct ServerInfo
    {
        public string Name
        {
            get;
            private set;
        }

        public ServerInfo(string name)
        {
            Name = name;
            ClientSerial = 0;
        }

        public long ClientSerial;
    }
}
