using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore
{
    public class Zip<T1, T2> : IEnumerable<(T1, T2)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;

        private struct Enumerator : IEnumerator<(T1, T2)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;

            public (T1, T2) Current => (
                m_t1.Current, 
                m_t2.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1, 
                IEnumerator<T2> t2
                )
            {
                m_t1 = t1;
                m_t2 = t2;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
            }

            public bool MoveNext()
            {
                return 
                    m_t1.MoveNext() && 
                    m_t2.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1, 
            IEnumerable<T2> t2
            )
        {
            m_t1 = t1;
            m_t2 = t2;
        }

        public IEnumerator<(T1, T2)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(), 
                m_t2.GetEnumerator()
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3> : IEnumerable<(T1, T2, T3)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;

        private struct Enumerator : IEnumerator<(T1, T2, T3)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;

            public (T1, T2, T3) Current => (
                m_t1.Current, 
                m_t2.Current, 
                m_t3.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1, 
                IEnumerator<T2> t2, 
                IEnumerator<T3> t3
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
            }

            public bool MoveNext()
            {
                return 
                    m_t1.MoveNext() && 
                    m_t2.MoveNext() && 
                    m_t3.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1, 
            IEnumerable<T2> t2, 
            IEnumerable<T3> t3
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
        }

        public IEnumerator<(T1, T2, T3)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(), 
                m_t2.GetEnumerator(), 
                m_t3.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3, T4> : IEnumerable<(T1, T2, T3, T4)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;
        private readonly IEnumerable<T4> m_t4;

        private struct Enumerator : IEnumerator<(T1, T2, T3, T4)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;
            private readonly IEnumerator<T4> m_t4;

            public (T1, T2, T3, T4) Current => (
                m_t1.Current, 
                m_t2.Current, 
                m_t3.Current, 
                m_t4.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1, 
                IEnumerator<T2> t2, 
                IEnumerator<T3> t3, 
                IEnumerator<T4> t4
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
                m_t4 = t4;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
                m_t4.Dispose();
            }

            public bool MoveNext()
            {
                return 
                    m_t1.MoveNext() && 
                    m_t2.MoveNext() && 
                    m_t3.MoveNext() && 
                    m_t4.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
                m_t4.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1, 
            IEnumerable<T2> t2, 
            IEnumerable<T3> t3, 
            IEnumerable<T4> t4
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
            m_t4 = t4;
        }

        public IEnumerator<(T1, T2, T3, T4)> GetEnumerator()
        {
            return new Enumerator(m_t1.GetEnumerator(), m_t2.GetEnumerator(), m_t3.GetEnumerator(), m_t4.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3, T4, T5> : IEnumerable<(T1, T2, T3, T4, T5)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;
        private readonly IEnumerable<T4> m_t4;
        private readonly IEnumerable<T5> m_t5;

        private struct Enumerator : IEnumerator<(T1, T2, T3, T4, T5)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;
            private readonly IEnumerator<T4> m_t4;
            private readonly IEnumerator<T5> m_t5;

            public (T1, T2, T3, T4, T5) Current => (
                m_t1.Current, 
                m_t2.Current,
                m_t3.Current, 
                m_t4.Current, 
                m_t5.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1, 
                IEnumerator<T2> t2, 
                IEnumerator<T3> t3, 
                IEnumerator<T4> t4,
                IEnumerator<T5> t5
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
                m_t4 = t4;
                m_t5 = t5;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
                m_t4.Dispose();
                m_t5.Dispose();
            }

            public bool MoveNext()
            {
                return 
                    m_t1.MoveNext() && 
                    m_t2.MoveNext() && 
                    m_t3.MoveNext() && 
                    m_t4.MoveNext() &&
                    m_t5.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
                m_t4.Reset();
                m_t5.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1, 
            IEnumerable<T2> t2, 
            IEnumerable<T3> t3, 
            IEnumerable<T4> t4,
            IEnumerable<T5> t5
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
            m_t4 = t4;
            m_t5 = t5;
        }

        public IEnumerator<(T1, T2, T3, T4, T5)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(), 
                m_t2.GetEnumerator(), 
                m_t3.GetEnumerator(), 
                m_t4.GetEnumerator(),
                m_t5.GetEnumerator()
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3, T4, T5, T6> : IEnumerable<(T1, T2, T3, T4, T5, T6)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;
        private readonly IEnumerable<T4> m_t4;
        private readonly IEnumerable<T5> m_t5;
        private readonly IEnumerable<T6> m_t6;

        private struct Enumerator : IEnumerator<(T1, T2, T3, T4, T5, T6)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;
            private readonly IEnumerator<T4> m_t4;
            private readonly IEnumerator<T5> m_t5;
            private readonly IEnumerator<T6> m_t6;

            public (T1, T2, T3, T4, T5, T6) Current => (
                m_t1.Current,
                m_t2.Current,
                m_t3.Current,
                m_t4.Current,
                m_t5.Current,
                m_t6.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1,
                IEnumerator<T2> t2,
                IEnumerator<T3> t3,
                IEnumerator<T4> t4,
                IEnumerator<T5> t5,
                IEnumerator<T6> t6
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
                m_t4 = t4;
                m_t5 = t5;
                m_t6 = t6;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
                m_t4.Dispose();
                m_t5.Dispose();
                m_t6.Dispose();
            }

            public bool MoveNext()
            {
                return
                    m_t1.MoveNext() &&
                    m_t2.MoveNext() &&
                    m_t3.MoveNext() &&
                    m_t4.MoveNext() &&
                    m_t5.MoveNext() &&
                    m_t6.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
                m_t4.Reset();
                m_t5.Reset();
                m_t6.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1,
            IEnumerable<T2> t2,
            IEnumerable<T3> t3,
            IEnumerable<T4> t4,
            IEnumerable<T5> t5,
            IEnumerable<T6> t6
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
            m_t4 = t4;
            m_t5 = t5;
            m_t6 = t6;
        }

        public IEnumerator<(T1, T2, T3, T4, T5, T6)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(),
                m_t2.GetEnumerator(),
                m_t3.GetEnumerator(),
                m_t4.GetEnumerator(),
                m_t5.GetEnumerator(),
                m_t6.GetEnumerator()
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3, T4, T5, T6, T7> : IEnumerable<(T1, T2, T3, T4, T5, T6, T7)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;
        private readonly IEnumerable<T4> m_t4;
        private readonly IEnumerable<T5> m_t5;
        private readonly IEnumerable<T6> m_t6;
        private readonly IEnumerable<T7> m_t7;

        private struct Enumerator : IEnumerator<(T1, T2, T3, T4, T5, T6, T7)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;
            private readonly IEnumerator<T4> m_t4;
            private readonly IEnumerator<T5> m_t5;
            private readonly IEnumerator<T6> m_t6;
            private readonly IEnumerator<T7> m_t7;

            public (T1, T2, T3, T4, T5, T6, T7) Current => (
                m_t1.Current,
                m_t2.Current,
                m_t3.Current,
                m_t4.Current,
                m_t5.Current,
                m_t6.Current,
                m_t7.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1,
                IEnumerator<T2> t2,
                IEnumerator<T3> t3,
                IEnumerator<T4> t4,
                IEnumerator<T5> t5,
                IEnumerator<T6> t6,
                IEnumerator<T7> t7
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
                m_t4 = t4;
                m_t5 = t5;
                m_t6 = t6;
                m_t7 = t7;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
                m_t4.Dispose();
                m_t5.Dispose();
                m_t6.Dispose();
                m_t7.Dispose();
            }

            public bool MoveNext()
            {
                return
                    m_t1.MoveNext() &&
                    m_t2.MoveNext() &&
                    m_t3.MoveNext() &&
                    m_t4.MoveNext() &&
                    m_t5.MoveNext() &&
                    m_t6.MoveNext() &&
                    m_t7.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
                m_t4.Reset();
                m_t5.Reset();
                m_t6.Reset();
                m_t7.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1,
            IEnumerable<T2> t2,
            IEnumerable<T3> t3,
            IEnumerable<T4> t4,
            IEnumerable<T5> t5,
            IEnumerable<T6> t6,
            IEnumerable<T7> t7
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
            m_t4 = t4;
            m_t5 = t5;
            m_t6 = t6;
            m_t7 = t7;
        }

        public IEnumerator<(T1, T2, T3, T4, T5, T6, T7)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(),
                m_t2.GetEnumerator(),
                m_t3.GetEnumerator(),
                m_t4.GetEnumerator(),
                m_t5.GetEnumerator(),
                m_t6.GetEnumerator(),
                m_t7.GetEnumerator()
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zip<T1, T2, T3, T4, T5, T6, T7, T8> : IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8)>
    {
        private readonly IEnumerable<T1> m_t1;
        private readonly IEnumerable<T2> m_t2;
        private readonly IEnumerable<T3> m_t3;
        private readonly IEnumerable<T4> m_t4;
        private readonly IEnumerable<T5> m_t5;
        private readonly IEnumerable<T6> m_t6;
        private readonly IEnumerable<T7> m_t7;
        private readonly IEnumerable<T8> m_t8;

        private struct Enumerator : IEnumerator<(T1, T2, T3, T4, T5, T6, T7, T8)>
        {
            private readonly IEnumerator<T1> m_t1;
            private readonly IEnumerator<T2> m_t2;
            private readonly IEnumerator<T3> m_t3;
            private readonly IEnumerator<T4> m_t4;
            private readonly IEnumerator<T5> m_t5;
            private readonly IEnumerator<T6> m_t6;
            private readonly IEnumerator<T7> m_t7;
            private readonly IEnumerator<T8> m_t8;

            public (T1, T2, T3, T4, T5, T6, T7, T8) Current => (
                m_t1.Current,
                m_t2.Current,
                m_t3.Current,
                m_t4.Current,
                m_t5.Current,
                m_t6.Current,
                m_t7.Current,
                m_t8.Current
                );

            object IEnumerator.Current => Current;

            public Enumerator(
                IEnumerator<T1> t1,
                IEnumerator<T2> t2,
                IEnumerator<T3> t3,
                IEnumerator<T4> t4,
                IEnumerator<T5> t5,
                IEnumerator<T6> t6,
                IEnumerator<T7> t7,
                IEnumerator<T8> t8
                )
            {
                m_t1 = t1;
                m_t2 = t2;
                m_t3 = t3;
                m_t4 = t4;
                m_t5 = t5;
                m_t6 = t6;
                m_t7 = t7;
                m_t8 = t8;
            }

            public void Dispose()
            {
                m_t1.Dispose();
                m_t2.Dispose();
                m_t3.Dispose();
                m_t4.Dispose();
                m_t5.Dispose();
                m_t6.Dispose();
                m_t7.Dispose();
                m_t8.Dispose();
            }

            public bool MoveNext()
            {
                return
                    m_t1.MoveNext() &&
                    m_t2.MoveNext() &&
                    m_t3.MoveNext() &&
                    m_t4.MoveNext() &&
                    m_t5.MoveNext() &&
                    m_t6.MoveNext() &&
                    m_t7.MoveNext() &&
                    m_t8.MoveNext();
            }

            public void Reset()
            {
                m_t1.Reset();
                m_t2.Reset();
                m_t3.Reset();
                m_t4.Reset();
                m_t5.Reset();
                m_t6.Reset();
                m_t7.Reset();
                m_t8.Reset();
            }
        }

        public Zip(
            IEnumerable<T1> t1,
            IEnumerable<T2> t2,
            IEnumerable<T3> t3,
            IEnumerable<T4> t4,
            IEnumerable<T5> t5,
            IEnumerable<T6> t6,
            IEnumerable<T7> t7,
            IEnumerable<T8> t8
            )
        {
            m_t1 = t1;
            m_t2 = t2;
            m_t3 = t3;
            m_t4 = t4;
            m_t5 = t5;
            m_t6 = t6;
            m_t7 = t7;
            m_t8 = t8;
        }

        public IEnumerator<(T1, T2, T3, T4, T5, T6, T7, T8)> GetEnumerator()
        {
            return new Enumerator(
                m_t1.GetEnumerator(),
                m_t2.GetEnumerator(),
                m_t3.GetEnumerator(),
                m_t4.GetEnumerator(),
                m_t5.GetEnumerator(),
                m_t6.GetEnumerator(),
                m_t7.GetEnumerator(),
                m_t8.GetEnumerator()
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
