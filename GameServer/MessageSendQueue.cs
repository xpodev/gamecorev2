using GameCore.Multiplayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GameServer
{
    public class MessageSendQueue<T> where T : Enum
    {
        private Queue<MessageInfo<T>> pendingMessages;

        private Queue<MessageInfo<T>> messagesBuffer;

        public Queue<MessageInfo<T>> Queue
        {
            get
            {
                return pendingMessages;
            }
        }

        public int BufferCapacity
        {
            get;
            private set;
        }

        public MessageSendQueue(MessageSendQueueSettings settings)
        {
            BufferCapacity = settings.BufferCapacity;
            pendingMessages = new Queue<MessageInfo<T>>(BufferCapacity);
            messagesBuffer = new Queue<MessageInfo<T>>(BufferCapacity);
            BufferCapacity = BufferCapacity;
        }

        public void QueueMessage(MessageInfo<T> msg)
        {
            if (messagesBuffer.Count == BufferCapacity)
            {
                messagesBuffer.Dequeue();
            }
            messagesBuffer.Enqueue(msg);
        }

        public MessageInfo<T> DequeueMessage()
        {
            while (pendingMessages.Count == 0)
            {
                if (messagesBuffer.Count != 0)
                {
                    SwapBuffers();
                }
            }
            return pendingMessages.Dequeue();
        }

        public async Task<MessageInfo<T>> DequeueMessageAsync()
        {
            return await Task.Run(DequeueMessage);
        }

        private void SwapBuffers()
        {
            Queue<MessageInfo<T>> temp = pendingMessages;
            pendingMessages = messagesBuffer;
            messagesBuffer = pendingMessages;
        }
    }

    public struct MessageSendQueueSettings
    {
        [JsonProperty]
        public int BufferCapacity
        {
            get;
            private set;
        }

        public const int DefaultBufferCapacity = 50;

        public MessageSendQueueSettings(int bufferCapacity = DefaultBufferCapacity)
        {
            BufferCapacity = bufferCapacity;
        }
    }
}
