using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Serialization
{
    /// <summary>
    /// This interface provides generic methods for serializaion and deserialization of objects
    /// </summary>
    /// <typeparam name="SType">The output type of the serializer</typeparam>
    public interface ISerializer<SType>
    {
        SType Serialize<T>(T obj);

        T Deserialize<T>(SType serialized);
    }
}
