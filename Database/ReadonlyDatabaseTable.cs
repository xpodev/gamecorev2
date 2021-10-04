using System.Collections.Generic;

namespace GameCore.Database
{
    public class ReadonlyDatabaseTable<KeyT, ValueT>
    {
        private readonly Dictionary<KeyT, ValueT> table = new Dictionary<KeyT, ValueT>();

        public ReadonlyDatabaseTable(Dictionary<KeyT, ValueT> items)
        {
            foreach (var item in items)
            {
                table.Add(item.Key, item.Value);
            }
        }

        public ValueT this[KeyT key]
        {
            get { return table[key]; }
            set { table[key] = value; }
        }

        public ValueT Get(KeyT key) => table[key];

        public T Get<T>(KeyT key) where T : class => table[key] as T;

        public bool TryGet(KeyT key, out ValueT value) => table.TryGetValue(key, out value);

        public bool TryGet<T>(KeyT key, out T value) where T : class
        {
            bool result = table.TryGetValue(key, out ValueT temp);
            value = temp as T;
            return result;
        }

        public bool ContainsKey(KeyT key) => table.ContainsKey(key);

        public bool ContainsValue(ref ValueT value) => table.ContainsValue(value);
    }

    public class ReadonlyDatabaseTable<T> : ReadonlyDatabaseTable<string, T>
    {
        public ReadonlyDatabaseTable(Dictionary<string, T> items) : base(items) { }
    }

    public class ReadonlyDatabaseTable : ReadonlyDatabaseTable<object>
    {
        public ReadonlyDatabaseTable(Dictionary<string, object> items) : base(items) { }
    }
}
