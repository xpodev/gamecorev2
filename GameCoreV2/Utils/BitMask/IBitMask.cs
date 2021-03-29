using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore
{
    using BitValue = Int32;
    using BitPosition = Int32;

    /// <summary>
    /// An interface that gives the basic functionality for an easy to use bit mask.
    /// </summary>
    /// <typeparam name="T">The underlying type of the bit buffer.</typeparam>
    public interface IBitMask<T>
    {
        /// <summary>
        /// The buffer that holds the actual bits.
        /// </summary>
        T Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// returns the indexth bit.
        /// </summary>
        /// <param name="index">The 0 based index of the bit to get.</param>
        /// <returns>The value of the bit. Either 0 or 1.</returns>
        BitValue GetBit(BitPosition index);

        /// <summary>
        /// set the indexth bit to 1.
        /// </summary>
        /// <param name="index">The 0 based index of the bit to get.</param>
        void SetBit(BitPosition index);

        /// <summary>
        /// set the indexth bit to the specified value.
        /// </summary>
        /// <param name="index">The 0 based index of the bit to get.</param>
        /// <param name="value">The new value for the bit. If value is 0 then the bit is set to 0, and 1 otherwise.</param>
        void SetBit(BitPosition index, BitValue value);

        /// <summary>
        /// set the indexth bit to 0.
        /// </summary>
        /// <param name="index">The 0 based index of the bit to get.</param>
        void ResetBit(BitPosition index);

        /// <summary>
        /// Flips the bit at the given index.
        /// </summary>
        /// <param name="index">The 0 based index of the bit to flip.</param>
        void FlipBit(BitPosition index);
    }
}
