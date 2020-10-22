using System;

namespace GameCore.Multiplayer
{
    [Serializable]
    [Serialization.SerializeBinary]
    public struct Message
    {
        public long Serial
        {
            get;
            private set;
        }

        public int TypeId
        {
            get;
            private set;
        }

        public object Object
        {
            get;
            private set;
        }

        public long TimeStamp
        {
            get;
            private set;
        }

        public Message(int id, object obj, long serial)
        {
            TypeId = id;
            Object = obj;
            TimeStamp = Protocol.CurrentProtocol.Clock.GetTimeStamp();
            Serial = serial;
        }
    }
}
