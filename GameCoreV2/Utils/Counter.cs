using System.Collections;
using System.Collections.Generic;

namespace GameCore
{
    public class Counter : IEnumerable<int>
    {
        public struct Enumerator : IEnumerator<int>
        {
            private int m_start, m_step, m_current;

            public object Current => m_current;

            int IEnumerator<int>.Current => m_current;

            public Enumerator(int start, int step)
            {
                m_start = m_current = start;
                m_step = step;
            }

            public bool MoveNext()
            {
                m_current += m_step;
                return true;
            }

            public void Reset()
            {
                m_current = m_start;
            }

            public void Dispose()
            {

            }
        }

        private int Start { get; }

        private int Step { get; }

        public Counter() : this(0) { }

        public Counter(int start) : this(start, 1) { }

        public Counter(int start, int step)
        {
            Start = start;
            Step = step;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(Start, Step);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
