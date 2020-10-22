using System;
using System.Net;


namespace GameCore.Multiplayer
{
    interface ISendMessage<T> where T : Enum
    {
        void SendMessage(Message<T> msg, IPEndPoint endPoint);
    }
}
