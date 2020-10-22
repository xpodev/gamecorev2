using System;


namespace GameCore.Serialization
{
    /// <summary>
    /// Use this attribute to mark the class/struct/field/property as serializable for the binary serializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct)]
    public class SerializeBinaryAttribute : Attribute
    {
        
    }

    /// <summary>
    /// Use this attribute to mark a specific property in a serializable class as unseriaizable
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreSerializeBinaryAttribute : Attribute
    {

    }
}
