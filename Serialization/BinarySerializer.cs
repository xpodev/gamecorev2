using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;


namespace GameCore.Serialization
{
    public class BinarySerializer : ISerializer<byte[]>
    {

        // TODO: this is not a raw binary deserializer
        public object Deserialize(byte[] serialized)
        {
            if (serialized.Length == 0 || serialized.All(b => b == 0))
            {
                return default;
            }
            using (MemoryStream ms = new MemoryStream(serialized))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        // TODO: this is not a raw binary deserializer
        public T Deserialize<T>(byte[] serialized)
        {
            if (serialized.Length == 0 || serialized.All(b => b == 0))
            {
                return default;
            }
            using (MemoryStream ms = new MemoryStream(serialized))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }

        // TODO: this is not a raw binary serializer
        public byte[] Serialize<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
