using System;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class MethodReferenceExtensions
    {
        public static MethodReference MakeGeneric(this MethodReference method, TypeReference type, params TypeReference[] types)
        {
            GenericInstanceMethod result = new GenericInstanceMethod(method.MakeGeneric(type));
            result.GenericArguments.AddRange(types);
            return result;
        }

        public static MethodReference MakeGeneric(this MethodReference method, TypeReference type)
        {
            if (!type.IsGenericInstance)
                return method;

            if (type.Resolve() != method.DeclaringType)
                throw new ArgumentException($"Can't instantiate method generic instance from a type which is not a generic instance of its containing type");

            MethodReference result = new MethodReference(method.Name, method.ReturnType, method.Module.ImportReference(type));

            result.Parameters.AddRange(method.Parameters);
            result.HasThis = method.HasThis;

            return result;
        }
    }
}
