namespace GameCore
{
    /// <summary>
    /// The base class for all bit masks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BitMask<T> : IBitMask<T> where T : struct
    {
        /// <summary>
        /// Internal bit buffer.
        /// </summary>
        protected T buffer;

        public T Buffer
        {
            get
            {
                return buffer;
            }
            set
            {
                buffer = value;
            }
        }

        public abstract void FlipBit(int index);

        public abstract int GetBit(int index);

        public abstract void ResetBit(int index);

        public abstract void SetBit(int index);

        public abstract void SetBit(int index, int value);
    }
}
