using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class TypeReferenceExtensions
    {
        public static bool IsEqualTo(this TypeReference type, TypeReference other)
        {
            // todo: add more cases
            if (type.FullName != other.FullName) return false;
            if (type.Resolve() != other.Resolve()) return false;
            if (type.IsArray != other.IsArray) return false;

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
    }
}
