using System;
using System.Runtime.InteropServices;

namespace GameCore.Net
{
    // pack the struct on signle byte boundry (most packed)
    // any other value will cause data corruption unless sizeof(T) == Pack
    // or Pack = 0 && T is either int or uint
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MessageHeader<T> where T : struct, Enum
    {
        private T id;
        private int size;

        public MessageHeader(T id)
        {
            this.id = id;
            size = 0;
        }

        public T Id
        {
            get
            {
                return id;
            }
            internal set
            {
                id = value;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
            internal set
            {
                size = value;
            }
        }
    }
}
