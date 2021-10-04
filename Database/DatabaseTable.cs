using System.Collections.Generic;

namespace GameCore.Database
{
    public class DatabaseTable<KeyT, ValueT>
    {
        private readonly Dictionary<KeyT, ValueT> table = new Dictionary<KeyT, ValueT>();

        public DatabaseTable() { }

        public DatabaseTable(Dictionary<KeyT, ValueT> items)
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

        public void Add(KeyT key, ValueT value) => table.Add(key, value);

        public bool ContainsKey(KeyT key) => table.ContainsKey(key);

        public bool ContainsValue(ref ValueT value) => table.ContainsValue(value);

        public ReadonlyDatabaseTable<KeyT, ValueT> AsReadonly => new ReadonlyDatabaseTable<KeyT, ValueT>(table);
    }

    public class DatabaseTable<T> : DatabaseTable<string, T>
    {
        public DatabaseTable(Dictionary<string, T> items) : base(items) { }
    }

    public class DatabaseTable : DatabaseTable<object>
    {
        public DatabaseTable(Dictionary<string, object> items) : base(items) { }
    }
}
