using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using GameCore.Net.Sync.Internal;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync
{
    public enum RPCDispatchMode
    {
        JumpTable,
        Dictionary,
        Switch
    }

    public class RPCDispatcher
    {
        private List<SynchronizedMethodWrapper> m_methods = new List<SynchronizedMethodWrapper>();
        private List<MethodWrapper> m_dispatchers = new List<MethodWrapper>();

        internal TypeDefinition DeclaringType { get; }


        public RPCDispatcher(TypeDefinition declaringType)
        {
            DeclaringType = declaringType;
        }


        public void AddMethod(MethodReference method) => AddMethod(new SynchronizedMethodWrapper(method));

        internal void AddMethod(SynchronizedMethodWrapper method) => m_methods.Add(method);

        public MethodDefinition GenerateRPCDispathcerType(SynchronizationSettings settings)
        {
            foreach (SynchronizedMethodWrapper method in m_methods)
            {
                MethodDefinition dispatcher = method.CreateMethodDispacther(settings);
                DeclaringType.Methods.Add(dispatcher);
                m_dispatchers.Add(new MethodWrapper(dispatcher));
            }

            MethodDefinition dispatcherMethod = GenerateRPCDispatcherMethod(settings);
            DeclaringType.Methods.Add(dispatcherMethod);
            return dispatcherMethod;
        }

        public MethodDefinition GenerateRPCDispatcherMethod(SynchronizationSettings settings)
        {
            MethodDefinition method = new MethodDefinition("RPCDispacther", MethodAttributes.Static, DeclaringType.Module.TypeSystem.Boolean);
            method.Parameters.Add(new ParameterDefinition(settings.MessageSettings.MessageType));

            ILProcessor il = method.Body.GetILProcessor();

            Instruction endOfMethod = il.Create(OpCodes.Ret);

            List<Instruction> jumpTable = new List<Instruction>(m_methods.Count);

            foreach (MethodWrapper rpc in m_dispatchers)
            {
                // all RPC dispatchers should have exactly 1 parameter (the message)
                ILGenerator.GenerateLoadArgument(il, rpc.MethodReference.Parameters[0], jumpTable);
            }

            new MethodWrapper(settings.MessageSettings.MessageIDGetter).GenerateCall(il, method.Parameters);
            il.Emit(OpCodes.Switch, jumpTable.ToArray());
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Br, endOfMethod);

            foreach ((MethodWrapper rpc, Instruction label) in new Zip<MethodWrapper, Instruction>(m_dispatchers, jumpTable))
            {
                il.Body.Instructions.Add(label);
                rpc.GenerateCall(il);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Br, endOfMethod);
            }

            il.Body.Instructions.Add(endOfMethod);

            return method;
        }
    }
}
