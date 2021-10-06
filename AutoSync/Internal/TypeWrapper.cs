using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace GameCore.Net.Sync.Internal
{
    internal class TypeWrapper
    {
        public TypeDefinition AsDefinition => TypeReference.Resolve();

        public TypeReference TypeReference { get; }


        public TypeWrapper(TypeReference reference)
        {
            TypeReference = reference;
        }
    }
}
