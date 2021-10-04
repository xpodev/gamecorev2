namespace GameCore.Net.Sync
{
    public interface IUIDGenerator<T>
    {
        T GenerateUID();

        void Remove(T uid);
    }
}
