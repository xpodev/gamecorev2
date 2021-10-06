using System;
using System.Collections.Generic;
using GameCore.Net.Sync.Extensions;
using GameCore.Net.Sync.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GameCore.Net.Sync.Processors
{
    public class MethodProcessor : Processor<MethodDefinition>
    {
        public MethodProcessor(MethodDefinition method) : base(method) { }

        public override bool Process(SynchronizationSettings settings)
        {
            if (!Item.CheckAuthority(settings.Authority, settings.IncludeNonAuthorityMethods))
            {
                return false;
            }

            CustomAttribute syncAttribute = Item.GetCustomAttribute(typeof(SynchronizeCallAttribute));
            if (Item.GetAttribute(typeof(SynchronizeCallAttribute)) is SynchronizeCallAttribute sync && !sync.IsSynchronized)
            {
                syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.IsSynchronized),
                    new CustomAttributeArgument(Item.Module.TypeSystem.Boolean, true)
                    ));

                if (sync.Authority != settings.Authority)
                {
                    settings.RPCDispatcher.AddMethod(Item);
                    return true;
                }

                ILProcessor il = Item.Body.GetILProcessor();

                if (!sync.ExecuteOnAuthority && sync.Authority == settings.Authority)
                {
                    if (!Item.ReturnType.IsEqualTo(Item.Module.TypeSystem.Void))
                        Item.ReturnType = Item.Module.TypeSystem.Void;

                    il.Body.Instructions.Clear();
                    il.Emit(OpCodes.Ret);
                }

                List<Instruction> instructions = new List<Instruction>();

                Instruction onError = il.Create(OpCodes.Nop);

                VariableDefinition messageVariable = new VariableDefinition(settings.MessageSettings.MessageType);

                il.Body.Variables.Add(messageVariable);

                // creating the message
                {
                    if (Item.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute customCall)
                    {
                        ILGenerator.GenerateCustomFunctionCall(il, new CustomCallWrapper(customCall), new Dictionary<string, object>()
                        {
                            { "Id", sync.Id },
                            { "IsReliable", sync.Reliable },
                            { "Priority", sync.Priority },
                            { "IsMulticast", sync.Multicast }
                        }, instructions);
                    }
                    else
                    {
                        instructions.Add(ILGenerator.CreateLoadValue(il, sync.Id));
                    }
                    instructions.Add(il.Create(
                            settings.MessageSettings.MessageConstructor.IsConstructor ? OpCodes.Newobj : OpCodes.Call,
                            settings.MessageSettings.MessageConstructor
                            ));
                    ILGenerator.GenerateSetLocal(il, messageVariable, instructions);
                    //ILGenerator.GenerateLoadLocal(il, messageVariable, instructions);
                }

                Instruction nop = il.Create(OpCodes.Nop);
                int nopIndex = instructions.Count;
                instructions.Add(nop);

                /*
                 message = new Message()
                 for arg in args:
                    message.add(serialize(arg));
                 NetworkManager.send(message)
                 */
                if (Item.HasParameters)
                {
                    Dictionary<string, object> arguments = new Dictionary<string, object>();

                    foreach (ParameterDefinition parameter in Item.Parameters)
                    {
                        MethodReference serializer = settings.Serializers.GetSerializer(parameter.ParameterType).MethodReference;
                        if (serializer == null)
                        {
                            throw new Exception($"UnserializableType {parameter.ParameterType.FullName}");
                        }

                        MethodDefinition serializerDefinition = serializer.Resolve();

                        TypeSerializerAttribute serializerSpec = serializerDefinition.GetAttribute(typeof(TypeSerializerAttribute)) as TypeSerializerAttribute;
                        if (serializerDefinition.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute customCall)
                        {
                            arguments["Operation"] = (int)SerializationOperation.Serialize;
                            arguments["Object"] = parameter;
                            if (serializerSpec.Direct)
                                arguments["Message"] = messageVariable;
                            else
                                arguments.Remove("Message");
                            ILGenerator.GenerateCustomFunctionCall(il, new CustomCallWrapper(customCall), arguments, instructions);
                        } 
                        else
                        {
                            if (serializerSpec.Direct)
                                ILGenerator.GenerateLoadLocal(il, messageVariable, instructions);
                            ILGenerator.GenerateLoadArgument(il, parameter, instructions);
                        }

                        instructions.Add(il.Create(OpCodes.Call, Item.Module.ImportReference(serializer)));

                        if (settings.MessageSettings.MessageType.IsClassAssignableFrom(serializer.ReturnType.Resolve()))
                        {
                            continue;
                        }
                        else if (serializer.ReturnType.IsEqualTo(Item.Module.TypeSystem.Boolean))
                        {
                            instructions.Add(il.Create(OpCodes.Brfalse_S, onError));
                        } 
                        else if (serializer.ReturnType.IsEqualTo(Item.Module.TypeSystem.Void))
                        {
                            instructions.Insert(nopIndex + 1, il.Create(OpCodes.Dup));
                        } 
                        else
                        {
                            throw new Exception($"Invalid return type for direct serializer {serializer.FullName}: {serializer.ReturnType.FullName}");
                        }
                    }
                }

                {
                    if (settings.MessageSettings.MessageSenderMethod.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute customCall)
                    {
                        ILGenerator.GenerateCustomFunctionCall(il, new CustomCallWrapper(customCall), new Dictionary<string, object>()
                        {
                            { "Id", sync.Id },
                            { "IsReliable", sync.Reliable },
                            { "Priority", sync.Priority },
                            { "IsMulticast", sync.Multicast },
                            { "Message", messageVariable }
                        }, instructions);
                    }
                    else
                    {
                        ILGenerator.GenerateLoadLocal(il, messageVariable, instructions);
                        instructions.Add(ILGenerator.CreateLoadValue(il, sync.Reliable));
                        instructions.Add(ILGenerator.CreateLoadValue(il, sync.Multicast));
                    }
                    instructions.Add(il.Create(OpCodes.Call, settings.MessageSettings.MessageSenderMethod));
                }

                instructions.Add(onError);

                il.Body.Instructions.AddRangeAt(instructions, 0);

                return true;
            }
            return true;
        }
    }
}
