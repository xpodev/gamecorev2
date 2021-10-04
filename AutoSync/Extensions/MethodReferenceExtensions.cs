using System;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace GameCore.Net.Sync.Extensions
{
    public static class MethodReferenceExtensions
    {
        public static MethodReference MakeGeneric(this MethodReference m, params TypeReference[] types)
        {
            return m.Resolve().MakeGeneric(types);
        }
    }
}
