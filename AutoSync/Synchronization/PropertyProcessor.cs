using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Net.Sync.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GameCore.Net.Sync.Processors
{
    public class PropertyProcessor : Processor<PropertyDefinition>
    {
        public PropertyProcessor(PropertyDefinition prop) : base(prop) { }

        public override bool Process(SynchronizationSettings settings)
        {
            // todo: fix this function
            if (!Item.CheckAuthority(settings.Authority, settings.IncludeNonAuthorityMethods))
            {
                return false;
            }

            if (Item.GetAttribute(typeof(SynchronizeValueAttribute)) is SynchronizeValueAttribute sync && !sync.IsSynchronized)
            {
                //if (sync.Authority != settings.Authority)
                //{
                //    SynchronizeObjectAttribute.UIDGenerator.Remove(sync.Id);
                //    return false;
                //}

                if (Item.SetMethod != null)
                {
                    MethodDefinition syncedSet = Item.SetMethod;

                    ILProcessor il = syncedSet.Body.GetILProcessor();

                    Instruction nop = il.Create(OpCodes.Nop);
                    Instruction branch = il.Create(OpCodes.Brfalse_S, nop);

                    Instruction currentValueInst1 = il.Create(OpCodes.Nop);
                    Instruction currentValueInst2 = il.Create(OpCodes.Nop);

                    if (!sync.ExecuteOnAuthority && sync.Authority == settings.Authority)
                    {
                        il.Body.Instructions.Clear();
                        il.Emit(OpCodes.Ret);
                    }

                    List<Instruction> instructions = new List<Instruction>();

                    if (Item.GetMethod != null)
                    {
                        instructions.AddRange(new Instruction[]
                        {
                            il.Create(OpCodes.Ldarg_0),
                            il.Create(Item.GetMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, Item.GetMethod),
                            currentValueInst1,
                            currentValueInst2,
                            il.Create(OpCodes.Ldarg_1),
                            il.Create(OpCodes.Ceq),
                            il.Create(OpCodes.Ldc_I4_0),
                            il.Create(OpCodes.Ceq),
                            branch
                        });
                    }

                    if (sync.ConditionFunction != null)
                    {
                        MethodReference condition = Item.DeclaringType.Methods.SingleOrDefault(m =>
                        m.Name == sync.ConditionFunction &&
                        m.ReturnType == Item.Module.TypeSystem.Boolean &&
                        m.Parameters.Count <= 2 &&
                        (m.IsStatic || m.IsStatic == Item.SetMethod.IsStatic)
                         );

                        if (condition != null)
                        {
                            MethodDefinition conditionDef = condition.Resolve();
                            if (!conditionDef.IsStatic)
                                instructions.Add(il.Create(OpCodes.Ldarg_0));
                            if (condition.HasGenericParameters)
                                condition = condition.MakeGeneric(Item.DeclaringType, new TypeReference[] { Item.PropertyType });
                            switch (condition.Parameters.Count)
                            {
                                case 2:
                                    VariableDefinition local = new VariableDefinition(Item.GetMethod.ReturnType);

                                    currentValueInst1.OpCode = OpCodes.Stloc;
                                    currentValueInst1.Operand = local;
                                    currentValueInst2.OpCode = local.VariableType.IsByReference ? OpCodes.Ldloca : OpCodes.Ldloc;
                                    currentValueInst2.Operand = local;

                                    il.Body.Variables.Add(local);

                                    if (condition.Parameters[0].ParameterType.IsByReference)
                                        instructions.Add(il.Create(OpCodes.Ldarga, 1));
                                    else
                                        instructions.Add(il.Create(OpCodes.Ldarg_1));
                                    instructions.Add(il.Create(
                                        condition.Parameters[1].ParameterType.IsByReference ? OpCodes.Ldloca : OpCodes.Ldloc, local
                                        ));

                                    break;
                                case 1:
                                    instructions.Add(il.Create(OpCodes.Ldarg_1));
                                    instructions.Remove(currentValueInst1);
                                    instructions.Remove(currentValueInst2);
                                    break;
                                case 0:
                                default:
                                    instructions.Remove(currentValueInst1);
                                    instructions.Remove(currentValueInst2);
                                    break;
                            }
                            instructions.AddRange(new Instruction[] {
                                il.Create(conditionDef.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, condition),
                                il.Create(OpCodes.Brfalse, nop)
                                });
                        }
                        else
                        {
                            if (Item.SetMethod.IsStatic)
                            {
                                throw new System.MissingMethodException($"Property {Item.DeclaringType.FullName}.{Item.Name}: Could not find method 'static bool {sync.ConditionFunction}()' with 0 to 2 parameters of type {Item.PropertyType.FullName}");
                            }
                            else
                            {
                                throw new System.MissingMethodException($"Property {Item.DeclaringType.FullName}.{Item.Name}: Could not find method 'bool {sync.ConditionFunction}()' or 'static bool {sync.ConditionFunction}()' with 0 to 2 parameters of type {Item.PropertyType.FullName}");
                            }
                        }
                    }

                    //// todo: fix
                    //instructions.AddRange(new Instruction[]{
                    //    il.Create(OpCodes.Ldarg_0),
                    //    il.Create(OpCodes.Ldfld, PendingUpdatesField),
                    //    il.Create(OpCodes.Ldc_I4, (int)CommandType.Set),
                    //    il.Create(OpCodes.Ldc_I4, sync.Id),
                    //    il.Create(OpCodes.Newobj, Item.Module.ImportReference(typeof(Command<int>).GetConstructor(new []
                    //    {
                    //        typeof(CommandType),
                    //        typeof(int)
                    //    }))),
                    //    il.Create(OpCodes.Callvirt, Item.Module.ImportReference(typeof(List<Command<int>>).GetMethod("Add"))),
                    //    nop
                    //});

                    il.Body.Instructions.AddRangeAt(instructions, 0);
                }
            }
            return false;
        }
    }
}
