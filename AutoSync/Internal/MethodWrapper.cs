using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Internal
{
    internal class MethodWrapper
    {
        public MethodDefinition AsDefinition => MethodReference.Resolve();

        public CustomCallWrapper CustomCall { get; }

        public bool HasCustomCall => CustomCall != null;

        public bool IsStatic => AsDefinition.IsStatic;

        public bool IsVirtual => AsDefinition.IsVirtual;

        public MethodReference MethodReference { get; }


        public MethodWrapper(MethodReference reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            MethodReference = reference;

            CustomCall = AsDefinition.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute customCall ?
                new CustomCallWrapper(customCall) : null;
        }

        /// <summary>
        /// Generated a call to the method in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>).
        /// This function assumes all the parameters are already on the stack.
        /// </summary>
        /// <param name="il">The processor to use in order to create the instruction</param>
        public void GenerateCall(ILProcessor il, IList<Instruction> instructions = null)
        {
            instructions = instructions ?? il.Body.Instructions;
            instructions.Add(il.Create(IsVirtual ? OpCodes.Callvirt : OpCodes.Call, MethodReference));
        }

        /// <summary>
        /// Generated a call to the method in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>) using custom arguments.
        /// </summary>
        /// <param name="il">The processor to use in order to create the instruction</param>
        public void GenerateCall(ILProcessor il, IDictionary<string, object> arguments, IList<Instruction> instructions = null)
        {
            instructions = instructions ?? il.Body.Instructions;

            if (HasCustomCall)
                ILGenerator.GenerateCustomFunctionCall(il, CustomCall, arguments, instructions);

            instructions.Add(il.Create(IsVirtual ? OpCodes.Callvirt : OpCodes.Call, MethodReference));
        }

        /// <summary>
        /// Generated a call to the method in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>) using the given arguments.
        /// </summary>
        /// <param name="il">The processor to use in order to create the instruction</param>
        public void GenerateCall(ILProcessor il, IEnumerable<object> arguments, IList<Instruction> instructions = null)
        {
            instructions = instructions ?? il.Body.Instructions;

            ILGenerator.GenerateFunctionCall(il, arguments, instructions);

            instructions.Add(il.Create(IsVirtual ? OpCodes.Callvirt : OpCodes.Call, MethodReference));
        }
    }
}
