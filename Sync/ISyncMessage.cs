namespace GameCore.Net
{
    public interface ISyncMessage<T> where T : struct
    {
        T[] ArgumentIds { get; }

        byte[] Data { get; }
    }
}
