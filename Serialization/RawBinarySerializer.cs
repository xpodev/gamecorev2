using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GameCore.Serialization
{
    public sealed class RawBinarySerializer : ISerializer<byte[]>
    {
        private readonly MethodInfo m_fSerializeStruct = typeof(RawBinarySerializer).GetMethod(nameof(DeserializeStruct));
        private readonly MethodInfo m_fDeserializeStruct = typeof(RawBinarySerializer).GetMethod(nameof(DeserializeStruct));
        private readonly MethodInfo m_fDeserializeCustom = typeof(RawBinarySerializer).GetMethod(nameof(DeserializeStruct));

        public ISerializer<byte[]> DefaultSerializer
        {
            get; set;
        }

        public T Deserialize<T>(byte[] serialized)
        {
            Type type = typeof(T);
            if (type.IsValueType)
            {
                return (T)m_fDeserializeStruct.Invoke(this, new object[] { serialized });
            }
            else if (type.IsClass)
            {
                if (typeof(CustomRawBinarySerializer).IsAssignableFrom(type))
                {
                    return (T)m_fDeserializeCustom.Invoke(this, new object[] { serialized });
                }
            }
            if (DefaultSerializer is null)
            {
                throw new ArgumentException($"Can\'t serialize \'byte[]\' to type \'{type.FullName}\'");
            }
            return DefaultSerializer.Deserialize<T>(serialized);
        }

        private T DeserializeStruct<T>(byte[] serialized) where T : struct
        {
            return serialized.FromBytes<T>();
        }

        private T DeserializeCustom<T>(byte[] serialized) where T : CustomRawBinarySerializer, new()
        {
            T result = new T();
            result.Deserialize(serialized);
            return result;
        }

        public byte[] Serialize<T>(T obj)
        {
            Type type = typeof(T);
            if (type.IsValueType)
            {
                return (byte[])m_fSerializeStruct.Invoke(this, new object[] { obj });
            } else if (type.IsClass)
            {
                CustomRawBinarySerializer custom = obj as CustomRawBinarySerializer;
#if NET5_0
                if (custom is not null)
#else
                if (custom != null)
#endif
                {
                    return SerializeCustom(custom);
                }
            }
            return DefaultSerializer?.Serialize(obj) ?? throw new ArgumentException($"Can\'t serialize \'obj\' of type \'{type.FullName}\'");
        }

        private byte[] SerializeStruct<T>(T obj) where T : struct
        {
            return obj.GetBytes();
        }

        private byte[] SerializeCustom(CustomRawBinarySerializer obj)
        {
            return obj.Serialize();
        }
    }
}
