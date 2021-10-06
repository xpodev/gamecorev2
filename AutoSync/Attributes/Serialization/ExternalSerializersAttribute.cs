using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ExternalSerializersAttribute : Attribute
    {
        public Type Type { get; }

        public ExternalSerializersAttribute(Type type)
        {
            Type = type;
        }
    }
}
