using System;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class TypeReferenceExtensions
    {
        public static bool IsEqualTo(this TypeReference type, TypeReference other)
        {
            // todo: add more cases
            if (type.FullName != other.FullName) return false;
            if (type.IsArray != other.IsArray) return false;
            if (type.IsByReference != other.IsByReference) return false;
            if (type.Resolve().AssemblyQualifiedName() != other.Resolve().AssemblyQualifiedName()) return false;
            //if (type.Resolve() != other.Resolve()) return false;

            return true;
        }

        public static TypeReference MakeGeneric(this TypeReference type, params TypeReference[] types)
        {
            if (type.GenericParameters.Count != types.Length)
                throw new ArgumentException($"Types count must be the same as the type parameter count");

            GenericInstanceType result = new GenericInstanceType(type);
            result.GenericArguments.AddRange(types);
            return result;
        }

        public static (MethodDefinition, TypeReference) GetMethodInHierarchy(this TypeReference type, string name, params TypeReference[] argsTypes)
        {
            MethodDefinition method;
            do
            {
                if ((method = type.Resolve().GetMethod(name, argsTypes)) != null) break;
            } while ((type = type.Resolve().BaseType) != null);
            return (method, type);
        }

        public static (MethodDefinition, TypeReference) GetMethodInHierarchy(this TypeReference type, string name)
        {
            MethodDefinition method;
            do
            {
                if ((method = type.Resolve().GetMethod(name)) != null) break;
            } while ((type = type.Resolve().BaseType) != null);
            return (method, type);
        }

        public static (PropertyDefinition, TypeReference) GetPropertyInHierarchy(this TypeReference type, string name, TypeReference pType = null)
        {
            PropertyDefinition property;
            do
            {
                if ((property = type.Resolve().GetProperty(name, pType)) != null) break;
            } while ((type = type.Resolve().BaseType) != null);
            return (property, type);
        }

    }
}
