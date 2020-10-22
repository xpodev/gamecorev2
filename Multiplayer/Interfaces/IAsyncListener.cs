using System;


namespace GameCore.Multiplayer
{
    interface IAsyncListener
    {
        event Action<Message> OnReceive;

        bool IsRunning
        {
            get;
        }

        void BeginReceiveLoop(string threadName = null);

        void ReceiveLoop();

        void Stop();
    }
}
