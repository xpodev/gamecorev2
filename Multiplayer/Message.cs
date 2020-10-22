using System;
using System.Net;

namespace GameCore.Multiplayer
{
    [Serializable]
    [Serialization.SerializeBinary]
    public class Message<T> where T : Enum
    {
        public long Serial
        {
            get;
            private set;
        }

        public T CommandId
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

        public Message(T id, object obj, long serial)
        {
            CommandId = id;
            Object = obj;
            TimeStamp = Protocol<T>.CurrentProtocol.Clock.GetTimeStamp();
            Serial = serial;
        }
    }

    public class MessageInfo<T> where T : Enum
    {
        public Message<T> Message
        {
            get;
            private set;
        }

        public IPEndPoint EndPoint
        {
            get;
            private set;
        }

        public MessageInfo(Message<T> msg, IPEndPoint endPoint)
        {
            Message = msg;
            EndPoint = endPoint;
        }
    }
}
