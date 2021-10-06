using System;
using System.Collections.Generic;
using GameCore.Net.Sync.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GameCore.Net.Sync.Internal
{
    internal class SynchronizedMethodWrapper : MethodWrapper
    {
        public Authority Authority { get; }

        public bool ExecuteOnAuthority { get; }

        public bool IsMulticastCall { get; }

        public bool IsReliableCall { get; }

        public bool IsSynchronized { get; }

        public float Priority { get; }

        public object ObjectId { get; }


        public SynchronizedMethodWrapper(MethodReference reference) : base(reference)
        {
            if (!(AsDefinition.GetAttribute(typeof(SynchronizeCallAttribute)) is SynchronizeCallAttribute sync))
                throw new ArgumentException($"The referenced method must have (a subclass) {nameof(SynchronizeCallAttribute)} attribute");

            if (reference.HasGenericParameters)
                throw new ArgumentException($"A synchronized method may not be generic");

            Authority = sync.Authority;
            ExecuteOnAuthority = sync.ExecuteOnAuthority;
            IsMulticastCall = sync.Multicast;
            IsReliableCall = sync.Reliable;
            IsSynchronized = sync.IsSynchronized;
            ObjectId = sync.Id;
            Priority = sync.Priority;
        }

        internal MethodDefinition CreateMethodDispacther(SynchronizationSettings settings)
        {
            MethodDefinition method = new MethodDefinition(
                $"<{MethodReference.FullName.Replace('.', '_') }> m__RPCDispatcher", 
                MethodAttributes.Static, 
                MethodReference.Module.TypeSystem.Void
                );

            ParameterDefinition messageParameter = new ParameterDefinition(settings.MessageSettings.MessageType);
            method.Parameters.Add(messageParameter);

            ILProcessor il = method.Body.GetILProcessor();

            if (!IsStatic)
            {
                // todo: generate code for instance access
            }

            foreach (ParameterDefinition parameter in MethodReference.Parameters)
            {
                SerializationMethodWrapper deserializer = settings.SerializationTable.GetDeserializer(parameter.ParameterType);

                if (deserializer == null)
                    // todo: replace with actual UnserializableType exception for deserialization
                    throw new Exception($"UnserializableType: {parameter.ParameterType.FullName}");

                if (deserializer.HasCustomCall)
                {
                    // todo: add more custom arguments
                    deserializer.GenerateCall(il, new Dictionary<string, object>()
                    {
                        { "Message", messageParameter }
                    });
                }
                else
                {
                    ILGenerator.GenerateLoadArgument(il, messageParameter);
                    deserializer.GenerateCall(il);
                }
            }

            GenerateCall(il);

            il.Emit(OpCodes.Ret);

            return method;
        }
    }
}
