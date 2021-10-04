namespace GameCore.Net
{
    public interface ISyncMessageQueueItem<T> where T : struct
    {
        float Priority { get; }

        float InsertionTime { get; }

        ISyncMessage<T> Message { get; }
    }
}
