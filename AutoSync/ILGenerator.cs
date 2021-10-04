using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace GameCore.Net.Sync.Generators
{
    public class ILGenerator
    {
        public static Instruction CreateLoadValue(ILProcessor il, object value)
        {
            if (value is null) return il.Create(OpCodes.Ldnull);
            else if (value is string s) return il.Create(OpCodes.Ldstr, s);
            else if (value is char c) return il.Create(OpCodes.Ldc_I4, c);
            else if (value is byte b) return il.Create(OpCodes.Ldc_I4, b);
            else if (value is sbyte sb) return il.Create(OpCodes.Ldc_I4, sb);
            else if (value is short sh) return il.Create(OpCodes.Ldc_I4, sh);
            else if (value is ushort us) return il.Create(OpCodes.Ldc_I4, us);
            else if (value is int i) return il.Create(OpCodes.Ldc_I4, i);
            else if (value is uint u) return il.Create(OpCodes.Ldc_I4, u);
            else if (value is long l) return il.Create(OpCodes.Ldc_I8, l);
            else if (value is ulong ul) return il.Create(OpCodes.Ldc_I8, ul);
            else if (value is float f) return il.Create(OpCodes.Ldc_R4, f);
            else if (value is double d) return il.Create(OpCodes.Ldc_R8, d);
            else throw new ArgumentException($"Got an unexpected argument {value}", nameof(value));
        }

        public static void GenerateCustomFunctionCall(ILProcessor il, CustomFunctionCallAttribute custom, IDictionary<string, object> arguments, IList<Instruction> insns = null)
        {
            if (arguments != null)
            {
                foreach ((int i, object item) in new Enumerate<object>(custom?.Args ?? Array.Empty<object>()))
                {
                    if (item is string name && arguments.TryGetValue(name, out object obj))
                    {
                        custom.Args[i] = obj;
                    }
                }
            }
            GenerateCustomFunctionCall(il, custom, insns);
        }

        public static void GenerateCustomFunctionCall(ILProcessor il, CustomFunctionCallAttribute custom, IList<Instruction> insns = null)
        {
            insns = insns ?? il.Body.Instructions;
            foreach (object item in custom?.Args ?? Array.Empty<object>())
            {
                insns.Add(CreateLoadValue(il, item));
            }
        }
    }
}
