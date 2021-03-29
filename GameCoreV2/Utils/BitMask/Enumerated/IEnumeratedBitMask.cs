using System;


namespace GameCore
{
    /// <summary>
    /// This class is used for accessing bits using members of an enum
    /// </summary>
    /// <typeparam name="T">The <c>enum</c> type to use to access individual bits</typeparam>
    public interface IEnumeratedBitMask<T> where T : struct, Enum
    {
        /// <summary>
        /// returns the indexth bit.
        /// </summary>
        /// <param name="index">The enum based index of the bit to get.</param>
        /// <returns>The value of the bit. Either 0 or 1.</returns>
        int GetBit(T index);

        /// <summary>
        /// set the indexth bit to 1.
        /// </summary>
        /// <param name="index">The enum based index of the bit to get.</param>
        void SetBit(T index);

        /// <summary>
        /// set the indexth bit to the specified value.
        /// </summary>
        /// <param name="index">The enum based index of the bit to get.</param>
        /// <param name="value">The new value for the bit. If value is 0 then the bit is set to 0, and 1 otherwise.</param>
        void SetBit(T index, int value);

        /// <summary>
        /// set the indexth bit to 0.
        /// </summary>
        /// <param name="index">The enum based index of the bit to get.</param>
        void ResetBit(T index);

        /// <summary>
        /// Flips the bit at the given index.
        /// </summary>
        /// <param name="index">The enum based index of the bit to flip.</param>
        void FlipBit(T index);
    }
}
