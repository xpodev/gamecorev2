using System.Collections;
using System.Collections.Generic;

namespace GameCore
{
    public class Range : IEnumerable<int>
    {
        public struct Enumerator : IEnumerator<int>
        {
            private int m_start, m_end, m_step, m_current;

            public object Current => m_current;

            int IEnumerator<int>.Current => m_current;

            public Enumerator(int start, int end, int step)
            {
                m_start = m_current = start;
                m_end = end;
                m_step = step;
            }

            public bool MoveNext()
            {
                m_current += m_step;
                return m_current < m_end;
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

        private int End { get; }

        private int Step { get; }

        public Range(int end) : this(0, end) { }

        public Range(int start, int end) : this(start, end, 1) { }

        public Range(int start, int end, int step)
        {
            Start = start;
            End = end;
            Step = step;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(Start, End, Step);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
