using System;


namespace GameCore.Multiplayer
{
    interface IAsyncListener<T> where T : Enum
    {
        event Protocol<T>.MessageHandler OnReceive;

        bool IsRunning
        {
            get;
        }

        void BeginReceiveLoop(string threadName = null);

        void ReceiveLoop();

        void Stop();
    }
}
