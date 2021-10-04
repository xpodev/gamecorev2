using System.Collections.Generic;
using Mono.Collections.Generic;

namespace GameCore.Net.Sync.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this Collection<T> c, IEnumerable<T> e)
        {
            foreach (T item in e)
            {
                c.Add(item);
            }
        }

        public static void AddRangeAt<T>(this Collection<T> c, IEnumerable<T> e, int index)
        {
            foreach (T item in e)
            {
                c.Insert(index++, item);
            }
        }

        public static Collection<T> Copy<T>(this Collection<T> c)
        {
            Collection<T> r = new Collection<T>(c.Count);
            for (int i = 0; i < c.Count; i++)
            {
                r.Add(c[i]);
            }
            return r;
        }

        public static Collection<T> Copy<T>(this Collection<T> c, System.Func<T, T> transformer)
        {
            Collection<T> r = new Collection<T>(c.Count);
            for (int i = 0; i < c.Count; i++)
            {
                r.Add(transformer(c[i]));
            }
            return r;
        }


        public static void CopyTo<T>(this Collection<T> c, Collection<T> other)
        {
            for (int i = 0; i < c.Count; i++)
            {
                other.Add(c[i]);
            }
        }

        public static void CopyTo<T>(this Collection<T> c, Collection<T> other, System.Func<T, T> transformer)
        {
            for (int i = 0; i < c.Count; i++)
            {
                other.Add(transformer(c[i]));
            }
        }
    }
}
