using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net.Sync
{
    public enum SerializationOperation
    {
        Serialize,
        Deserialize
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class SerializationSpecificationAttribute : Attribute
    {
        public bool Strict { get; set; }

        public bool Direct { get; set; }

        public virtual SerializationOperation Operation { get; }

        public Type Type { get; }

        public SerializationSpecificationAttribute(Type type)
        {
            Type = type;
        }
    }
}
