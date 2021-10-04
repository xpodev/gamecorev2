using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace GameCore.Net.Sync.Extensions
{
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        /// Is childTypeDef a subclass of parentTypeDef. Does not test interface inheritance
        /// </summary>
        /// <param name="childTypeDef"></param>
        /// <param name="parentTypeDef"></param>
        /// <returns></returns>
        public static bool IsSubclassOf(this TypeDefinition childTypeDef, TypeDefinition parentTypeDef) =>
           childTypeDef.MetadataToken
               != parentTypeDef.MetadataToken
               && childTypeDef
              .EnumerateBaseClasses()
              .Any(b => b.MetadataToken == parentTypeDef.MetadataToken);

        /// <summary>
        /// Enumerate the current type, it's parent and all the way to the top type
        /// </summary>
        /// <param name="klassType"></param>
        /// <returns></returns>
        public static IEnumerable<TypeDefinition> EnumerateBaseClasses(this TypeDefinition klassType)
        {
            for (var typeDefinition = klassType; typeDefinition != null; typeDefinition = typeDefinition.BaseType?.Resolve())
            {
                yield return typeDefinition;
            }
        }

        public static bool IsClassAssignableFrom(this TypeDefinition child, TypeDefinition parent)
        {
            return child == parent || child.IsSubclassOf(parent);
        }

        public static Collection<PropertyDefinition> AllProperties<T>(this TypeDefinition type)
        {
            System.Type baseType = typeof(T);
            Collection<PropertyDefinition> properties = new Collection<PropertyDefinition>();
            do
            {
                properties.AddRange(type.Properties);
            } while ((type = type.BaseType?.Resolve()) != null && type.FullName != baseType.FullName);
            return properties;
        }

        public static MethodDefinition GetMethod(this TypeDefinition type, string name, params TypeReference[] argsTypes)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == name && method.Parameters.Count == argsTypes.Length)
                {
                    for (int i = 0; i < argsTypes.Length; i++)
                    {
                        if (method.Parameters[i].ParameterType != argsTypes[i]) goto notEqual;
                    }
                    return method;
                }
            notEqual:
                continue;
            }
            return null;
        }

        public static PropertyDefinition GetProperty(this TypeDefinition type, string name, TypeReference pType = null)
        {
            foreach (PropertyDefinition property in type.Properties)
            {
                if (property.Name == name)
                {
                    if (pType == null) return property;
                    if (pType == property.PropertyType) return property;
                }
            }
            return null;
        }

        public static FieldDefinition GetField(this TypeDefinition type, string name, TypeReference fType = null)
        {
            foreach (FieldDefinition field in type.Fields)
            {
                if (field.Name == name)
                {
                    if (fType == null) return field;
                    if (fType == field.FieldType) return field;
                }
            }
            return null;
        }
    }
}
