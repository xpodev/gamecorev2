using System;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        public static MemberReference ImportReference<T>(this ModuleDefinition module, T item)
            where T : MemberReference
        {
            if (item is TypeReference type) return module.ImportReference(type);
            if (item is FieldReference field) return module.ImportReference(field);
            if (item is PropertyReference property) return module.ImportReference(property);
            if (item is MethodReference method) return module.ImportReference(method);
            throw new ArgumentException($"Incompatible type {typeof(T)}", nameof(item));
        }
    }
}
