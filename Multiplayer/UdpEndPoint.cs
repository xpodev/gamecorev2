using GameCore.Serialization;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace GameCore.Multiplayer
{
    class UdpEndPoint<T> : UdpClient, IAsyncListener<T>, ISendMessage<T> where T : Enum
    {
        public bool IsRunning
        {
            get;
            private set;
        }

        private class AsyncState
        {
            public bool Completed
            {
                get;
                private set;
            }

            public byte[] Buffer
            {
                get;
                private set;
            }

            public void Run()
            {
                if (!Completed)
                {
                    throw new InvalidOperationException("Tried to run a task that is already running");
                }
                Completed = false;
            }

            public void Complete()
            {
                if (Completed)
                {
                    throw new InvalidOperationException("Tried to complete a task that was already complete");
                }
                Completed = true;
            }

            public void SetupBuffer(int bufferSize)
            {
                if (!Completed)
                {
                    throw new InvalidOperationException("Tried to change the buffer while a task was running");
                }
                Buffer = new byte[bufferSize];
            }
        }

        public event Protocol<T>.MessageHandler OnReceive;

        private void BeginReceive(AsyncState state)
        {
            state.Run();
            byte[] buffer = new byte[Client.ReceiveBufferSize];
            Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out SocketError error, EndReceiveCallback, state);
            if (error != SocketError.Success)
            {
                throw new SocketException((int)error);
            }
        }

        private void EndReceiveCallback(IAsyncResult ar)
        {
            AsyncState state = (AsyncState)ar.AsyncState;
            state.Complete();

            BinarySerializer serializer = new BinarySerializer();
            Message<T> msg = serializer.Deserialize<Message<T>>(state.Buffer);

            if (OnReceive != null)
            {
                OnReceive.Invoke(msg);
            }
        }

        public void BeginReceiveLoop(string threadName = null)
        {
            Thread thread = new Thread(ReceiveLoop);
            if (threadName != null)
            {
                thread.Name = threadName;
            }
            thread.Start();
        }

        public void ReceiveLoop()
        {
            IsRunning = true;
            AsyncState state = new AsyncState();
            while (IsRunning)
            {
                if (state.Completed)
                {
                    BeginReceive(state);
                }
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void SendMessage(Message<T> msg, IPEndPoint endPoint)
        {
            BinarySerializer serializer = new BinarySerializer();
            byte[] data = serializer.Serialize(msg);
            Send(data, data.Length, endPoint);
        }
    }
}
