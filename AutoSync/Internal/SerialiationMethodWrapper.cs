using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCore.Net.Sync.Extensions;
using Mono.Cecil;

namespace GameCore.Net.Sync.Internal
{
    internal class SerializationMethodWrapper : MethodWrapper
    {
        public bool IsDirect { get; }

        public bool IsStrict { get; }

        public SerializationOperation SerializationOperation { get; }

        public TypeWrapper Type { get; }


        public SerializationMethodWrapper(MethodReference reference) : base(reference)
        {
            if (!(AsDefinition.GetAttribute(typeof(SerializationSpecificationAttribute)) is SerializationSpecificationAttribute spec))
                throw new ArgumentException($"The referenced method must have (a subclass) {nameof(SerializationSpecificationAttribute)} attribute");

            IsDirect = spec.Direct;
            IsStrict = spec.Strict;
            SerializationOperation = spec.Operation;
            Type = new TypeWrapper(reference.Module.ImportReference(spec.Type));
        }
    }
}
