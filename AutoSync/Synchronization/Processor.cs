namespace GameCore.Net.Sync.Processors
{
    public abstract class Processor<T>
    {
        public T Item { get; }

        public Processor(T item)
        {
            Item = item;
        }

        public abstract bool Process(SynchronizationSettings settings);
    }
}
