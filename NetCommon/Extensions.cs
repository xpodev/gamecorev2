using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.Net.Extensions
{
    public static class EnumExtension
    {
        internal static byte[] EnumGetBytes<EnumT>(this EnumT value) where EnumT : struct
        {
            byte[] bytes = default;
            Type enumType = Enum.GetUnderlyingType(typeof(EnumT));
            var v = Convert.ChangeType(value, enumType);
            if (enumType == typeof(char))
            {
#if NET5_0
                bytes = BitConverter.GetBytes((char)v).AsSpan(0, 1).ToArray();
#else
                bytes = BitConverter.GetBytes((char)v);
                Array.Resize(ref bytes, 1);
#endif
            }
            else if (enumType == typeof(byte))
            {
#if NET5_0
                bytes = BitConverter.GetBytes((byte)v).AsSpan(0, 1).ToArray();
#else
                bytes = BitConverter.GetBytes((byte)v);
                Array.Resize(ref bytes, 1);
#endif
            }
            else if (enumType == typeof(short))
            {
                bytes = BitConverter.GetBytes((short)v);
            }
            else if (enumType == typeof(ushort))
            {
                bytes = BitConverter.GetBytes((ushort)v);
            }
            else if (enumType == typeof(int))
            {
                bytes = BitConverter.GetBytes((int)v);
            }
            else if (enumType == typeof(uint))
            {
                bytes = BitConverter.GetBytes((uint)v);
            }
            else if (enumType == typeof(long))
            {
                bytes = BitConverter.GetBytes((long)v);
            }
            else if (enumType == typeof(ulong))
            {
                bytes = BitConverter.GetBytes((ulong)v);
            }
            return bytes;
        }

        internal static byte[] StructGetBytes<StructT>(this StructT value) where StructT : struct
        {
            int size = Marshal.SizeOf(value);
            byte[] bytes = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(value, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);

            Marshal.FreeHGlobal(ptr);

            return bytes;
        }

        public static byte[] GetBytes<T>(this T value) where T : struct
        {
            if (typeof(T).IsEnum)
            {
                return value.EnumGetBytes();
            }
            return value.StructGetBytes();
        }

        internal static EnumT EnumFromBytes<EnumT>(this IEnumerable<byte> bytes) where EnumT : struct
        {
            EnumT v = default;
            Type enumType = Enum.GetUnderlyingType(typeof(EnumT));
            if (enumType == typeof(char))
            {
                v = (EnumT)(object)bytes.StructFromBytes<char>();
            }
            else if (enumType == typeof(byte))
            {
                v = (EnumT)(object)bytes.StructFromBytes<byte>();
            }
            else if (enumType == typeof(short))
            {
                v = (EnumT)(object)bytes.StructFromBytes<short>();
            }
            else if (enumType == typeof(ushort))
            {
                v = (EnumT)(object)bytes.StructFromBytes<ushort>();
            }
            else if (enumType == typeof(int))
            {
                v = (EnumT)(object)bytes.StructFromBytes<int>();
            }
            else if (enumType == typeof(uint))
            {
                v = (EnumT)(object)bytes.StructFromBytes<uint>();
            }
            else if (enumType == typeof(long))
            {
                v = (EnumT)(object)bytes.StructFromBytes<long>();
            }
            else if (enumType == typeof(ulong))
            {
                v = (EnumT)(object)bytes.StructFromBytes<ulong>();
            }
            return v;
        }

        internal static StructT StructFromBytes<StructT>(this IEnumerable<byte> bytes) where StructT : struct
        {
            StructT outT = default;
            int size = Marshal.SizeOf(outT);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(new List<byte>(bytes).ToArray(), 0, ptr, size);
            outT = Marshal.PtrToStructure<StructT>(ptr);

            Marshal.FreeHGlobal(ptr);

            return outT;
        }

        internal static T FromBytes<T>(this IEnumerable<byte> bytes) where T : struct
        {
            if (typeof(T).IsEnum)
            {
                return bytes.EnumFromBytes<T>();
            }
            return bytes.StructFromBytes<T>();
        }
    }

}
