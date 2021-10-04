namespace GameCore.Net
{
    public interface ISyncMessageQueue<T> where T : struct
    {
        void Queue(ISyncMessageQueueItem<T> item);
    }
}
