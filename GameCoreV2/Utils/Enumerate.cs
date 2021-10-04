using System.Collections;
using System.Collections.Generic;

namespace GameCore
{
    public class Enumerate<T> : IEnumerable<(int, T)>
    {
        private readonly IEnumerable<T> m_enumerable;

        public struct Enumerator : IEnumerator<(int, T)>
        {
            private readonly IEnumerator<T> m_enumerator;

            private int m_currentIndex;

            public Enumerator(IEnumerator<T> enumerator)
            {
                m_enumerator = enumerator;
                m_currentIndex = -1;
            }

            public (int, T) Current => (m_currentIndex, m_enumerator.Current);

            object IEnumerator.Current => (m_currentIndex, m_enumerator.Current);

            public bool MoveNext()
            {
                ++m_currentIndex;
                return m_enumerator.MoveNext();
            }

            public void Reset()
            {
                m_currentIndex = -1;
                m_enumerator.Reset();
            }

            public void Dispose()
            {
                m_enumerator.Dispose();
            }
        }

        public Enumerate(IEnumerable<T> enumerable)
        {
            m_enumerable = enumerable;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_enumerable.GetEnumerator());
        }

        IEnumerator<(int, T)> IEnumerable<(int, T)>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct Enumerate : IEnumerable
    {
        private readonly IEnumerable m_enumerable;

        public struct Enumerator : IEnumerator
        {
            private readonly IEnumerator m_enumerator;

            private int m_currentIndex;

            public Enumerator(IEnumerator enumerator)
            {
                m_enumerator = enumerator;
                m_currentIndex = -1;
            }

            object IEnumerator.Current => (m_currentIndex, m_enumerator.Current);

            public bool MoveNext()
            {
                ++m_currentIndex;
                return m_enumerator.MoveNext();
            }

            public void Reset()
            {
                m_currentIndex = -1;
                m_enumerator.Reset();
            }
        }

        public Enumerate(IEnumerable enumerable)
        {
            m_enumerable = enumerable;
        }

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(m_enumerable.GetEnumerator());
    }
}
