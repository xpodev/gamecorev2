using System.Net;


namespace GameCore.Multiplayer
{
    interface ISendMessage
    {
        void SendMessage(Message msg, IPEndPoint endPoint);
    }
}
