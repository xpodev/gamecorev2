using System;


namespace GameCore
{
    using BitMaskBufferType = Int32;

    public class BitMask32 : BitMask<BitMaskBufferType>
    {
        public override void FlipBit(int index)
        {
            buffer ^= (BitMaskBufferType)(1 << index);
        }

        public override int GetBit(int index)
        {
            return buffer & (1 << index);
        }

        public override void ResetBit(int index)
        {
            buffer &= (BitMaskBufferType)~(1 << index);
        }

        public override void SetBit(int index)
        {
            buffer |= (BitMaskBufferType)(1 << index);
        }

        public override void SetBit(int index, int value)
        {
            if (value == 0)
            {
                ResetBit(index);
            }
            else
            {
                SetBit(index);
            }
        }
    }
}
