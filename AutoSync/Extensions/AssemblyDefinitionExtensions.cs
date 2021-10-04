using System;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static CustomAttribute GetCustomAttribute(this AssemblyDefinition member, Type type)
        {
            TypeDefinition attrDefinition = member.MainModule.ImportReference(type).Resolve();
            foreach (CustomAttribute attribute in member.CustomAttributes)
            {
                if (attribute.AttributeType.Resolve().IsClassAssignableFrom(attrDefinition))
                {
                    return attribute;
                }
            }
            return null;
        }

        public static object GetAttribute(this AssemblyDefinition member, Type type)
        {
            return member.GetCustomAttribute(type).ToAttributeObject(type);
        }

        public static bool HasAttribute(this AssemblyDefinition member, Type type)
        {
            return member.GetAttribute(type) != null;
        }

        public static CustomAttribute GetCustomAttribute<T>(this AssemblyDefinition member) where T : Attribute
        {
            TypeDefinition attrDefinition = member.MainModule.ImportReference(typeof(T)).Resolve();
            foreach (CustomAttribute attribute in member.CustomAttributes)
            {
                if (attribute.AttributeType.Resolve().IsClassAssignableFrom(attrDefinition))
                {
                    return attribute;
                }
            }
            return null;
        }

        public static T GetAttribute<T>(this AssemblyDefinition member) where T : Attribute
        {
            return member.GetCustomAttribute<T>().ToAttributeObject<T>();
        }

        public static bool HasAttribute<T>(this AssemblyDefinition member) where T : Attribute
        {
            return member.GetAttribute<T>() != null;
        }
    }
}
