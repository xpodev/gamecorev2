using System;

namespace GameCore.Net.Sync
{
    public class TypeDeserializerAttribute : SerializationSpecificationAttribute
    {
        public override SerializationOperation Operation => SerializationOperation.Deserialize;

        public TypeDeserializerAttribute(Type type) : base(type)
        {

        }
    }
}
