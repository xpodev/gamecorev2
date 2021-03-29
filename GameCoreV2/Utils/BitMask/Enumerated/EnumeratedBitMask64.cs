using System;


namespace GameCore
{
    public class EnumeratedBitMask64<T> : BitMask64, IEnumeratedBitMask<T> where T : struct, Enum
    {
        public void FlipBit(T index)
        {
            FlipBit(EnumTools.GetEnumValueAs<int, T>(index));
        }

        public int GetBit(T index)
        {
            return GetBit(EnumTools.GetEnumValueAs<int, T>(index));
        }

        public void ResetBit(T index)
        {
            ResetBit(EnumTools.GetEnumValueAs<int, T>(index));
        }

        public void SetBit(T index)
        {
            SetBit(EnumTools.GetEnumValueAs<int, T>(index));
        }

        public void SetBit(T index, int value)
        {
            SetBit(EnumTools.GetEnumValueAs<int, T>(index), value);
        }
    }
}
