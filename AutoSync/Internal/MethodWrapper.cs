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

        public bool CheckCustomCall(IDictionary<string, TypeReference> parameters, bool strict)
        {
            if (parameters.Count != CustomCall.Arguments.Length) return false;
            List<TypeReference> types = new List<TypeReference>();
            foreach (string name in CustomCall.Arguments)
            {
                if (!parameters.TryGetValue(name, out TypeReference type)) return false;
                types.Add(type);
            }
            return CheckCall(types, strict);
        }

        public bool CheckCall(ICollection<TypeReference> types, bool strict)
        {
            if (types.Count != MethodReference.Parameters.Count) return false;
            if (strict)
                foreach ((TypeReference type, ParameterDefinition parameter) 
                    in new Zip<TypeReference, ParameterDefinition>(types, MethodReference.Parameters))
                {
                    if (!parameter.ParameterType.IsEqualTo(type)) return false;
                }
            else
                foreach ((TypeReference type, ParameterDefinition parameter)
                    in new Zip<TypeReference, ParameterDefinition>(types, MethodReference.Parameters))
                {
                    if (!parameter.ParameterType.Resolve().IsClassAssignableFrom(type.Resolve())) return false;
                }
            return true;
        }

        /// <summary>
        /// Generated a call to the method and stores the generated code in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>).
        /// This function assumes all the parameters are already on the stack.
        /// </summary>
        /// <param name="il">The processor to use in order to create the instruction</param>
        public void GenerateCall(ILProcessor il, IList<Instruction> instructions = null)
        {
            instructions = instructions ?? il.Body.Instructions;
            instructions.Add(il.Create(IsVirtual ? OpCodes.Callvirt : OpCodes.Call, MethodReference));
        }

        /// <summary>
        /// Generated a custom call to the method and stores the generated code in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>) using custom arguments.
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
        /// Generated a call to the method and stores the generated code in the given instruction list (or in the <paramref name="il"/> body if it is <c>null</c>) using the given arguments.
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
