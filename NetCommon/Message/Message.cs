using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameCore.Net.Extensions;

namespace GameCore.Net
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Message<T> where T : struct, Enum
    {
        MessageHeader<T> header;
        List<byte> data;

        public Message(T id)
        {
            header = new MessageHeader<T>(id);
            data = new List<byte>();
        }

        public int Length
        {
            get
            {
                return data.Count;
            }
        }

        public MessageHeader<T> Header
        {
            get
            {
                return header;
            }
        }

        public Message<T> Insert(CustomMessageHandler customInsert)
        {
            customInsert.InsertInto(this);
            return this;
        }

        public Message<T> Insert(byte[] buffer)
        {
            data.AddRange(buffer);
            return this;
        }

        public Message<T> Insert<InT>(InT value) where InT : struct
        {
            data.AddRange(value.GetBytes());
            return this;
        }

        public OutT Extract<OutT>() where OutT : CustomMessageHandler, new()
        {
            OutT customExtract = new OutT();
            customExtract.ExtractFrom(this);
            return customExtract;
        }

        public Message<T> Extract<OutT>(out OutT outT) where OutT : CustomMessageHandler, new()
        {
            outT = Extract<OutT>();
            return this;
        }

        public Message<T> Extract<OutT>(out OutT outT, int offset = -1) where OutT : struct
        {
            outT = Extract<OutT>(offset);
            return this;
        }

        public OutT Extract<OutT>(int offset = -1) where OutT : struct
        {
            OutT outT = default;
            int size;

            if (typeof(OutT).IsEnum)
            {
                size = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(OutT)));
            } else
            {
                size = Marshal.SizeOf(outT);
            }

            byte[] buffer = new byte[size];
            if (offset < 0)
            {
                offset = data.Count - size;
            }

            for (int i = 0; i < size; ++i)
            {
                buffer[i] = data[i + offset];
            }

            data.RemoveRange(offset, size);

            return buffer.FromBytes<OutT>();
        }

        public byte[] ExtractBytes(int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(data.ToArray(), data.Count - length, bytes, 0, length);
            return bytes;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[data.Count + Marshal.SizeOf(header)];

            int offset = 0;

            header.Size = bytes.Length;

            byte[] tmpBytes = header.Id.GetBytes();
            Array.Copy(tmpBytes, bytes, tmpBytes.Length);
            offset += tmpBytes.Length;

            tmpBytes = header.Size.GetBytes();
            Array.Copy(tmpBytes, 0, bytes, offset, tmpBytes.Length);
            offset += tmpBytes.Length;

            data.ToArray().CopyTo(bytes, offset);

            header.Size = data.Count;

            return bytes;
        }

        public static Message<T> FromBytes(byte[] bytes)
        {
            Message<T> message = new Message<T>
            {
                data = new List<byte>(bytes)
            };
            message.header.Id = message.Extract<T>(0);
            message.header.Size = message.Extract<int>(0) - Marshal.SizeOf(message.header);

            return message;
        }
    }
}
