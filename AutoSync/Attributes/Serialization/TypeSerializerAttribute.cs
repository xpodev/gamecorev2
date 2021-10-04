using System;

namespace GameCore.Net.Sync
{
    public class TypeSerializerAttribute : SerializationSpecificationAttribute
    {
        public override SerializationOperation Operation => SerializationOperation.Serialize;

        public TypeSerializerAttribute(Type type) : base(type)
        {

        }
    }
}
