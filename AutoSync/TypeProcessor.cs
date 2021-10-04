using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Generators
{
    public class TypeProcessor
    {
        public readonly TypeDefinition m_type;

        public TypeDefinition Type => m_type;

        public FieldDefinition PendingUpdatesField { get; }
        public FieldDefinition NetworkUpdateRateField { get; }

        public ModuleDefinition InjectedModule { get; set; }

        public TypeReference MessageType { get; set; }

        public TypeProcessor(TypeDefinition typeDefinition)
        {
            m_type = typeDefinition;
            const string PendingUpdatesFieldName = "<PendingUpdates>k__generatedField";
            const string NetworkUpdateRateFieldName = "<NetworkUpdateRate>k__generatedField";

            if ((PendingUpdatesField = m_type.Fields.Where(field =>
                    field.Name == PendingUpdatesFieldName &&
                    field.FieldType.FullName == typeof(List<Command<int>>).FullName).FirstOrDefault()
                ) == null)
            {
                PendingUpdatesField = new FieldDefinition(
                    PendingUpdatesFieldName,
                    FieldAttributes.Family,
                    m_type.Module.ImportReference(typeof(List<Command<int>>))
                    );
            }

            if ((NetworkUpdateRateField = m_type.Fields.Where(field =>
                field.Name == NetworkUpdateRateFieldName &&
                field.FieldType == m_type.Module.TypeSystem.Single).FirstOrDefault()
            ) == null)
            {
                NetworkUpdateRateField = new FieldDefinition(
                NetworkUpdateRateFieldName,
                FieldAttributes.Private,
                m_type.Module.TypeSystem.Single
                );
            }
        }

        public void MakeSynced(PropertyDefinition prop, Authority authority)
        {
            CustomAttribute syncAttribute = prop.GetCustomAttribute(typeof(SynchronizeValueAttribute));
            SynchronizeValueAttribute sync;
            if ((sync = syncAttribute.ToAttributeObject<SynchronizeValueAttribute>()) != null && !sync.IsSynchronized)
            {
                syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.IsSynchronized),
                    new CustomAttributeArgument(m_type.Module.TypeSystem.Boolean, true)
                    ));

                if (sync.Authority != authority)
                {
                    SynchronizeObjectAttribute.UIDGenerator.Remove(sync.Id);
                    return;
                }

                if (prop.SetMethod != null)
                {
                    MethodDefinition syncedSet = prop.SetMethod;

                    ILProcessor il = syncedSet.Body.GetILProcessor();

                    Instruction nop = il.Create(OpCodes.Nop);
                    Instruction branch = il.Create(OpCodes.Brfalse_S, nop);

                    Instruction currentValueInst1 = il.Create(OpCodes.Nop);
                    Instruction currentValueInst2 = il.Create(OpCodes.Nop);

                    if (!sync.ExecuteOnAuthority && sync.Authority == authority)
                    {
                        il.Body.Instructions.Clear();
                        il.Emit(OpCodes.Ret);
                    }

                    List<Instruction> instructions = new List<Instruction>();

                    if (prop.GetMethod != null)
                    {
                        instructions.AddRange(new Instruction[]
                        {
                            il.Create(OpCodes.Ldarg_0),
                            il.Create(prop.GetMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, prop.GetMethod),
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
                        MethodReference condition = m_type.Methods.SingleOrDefault(m =>
                        m.Name == sync.ConditionFunction &&
                        m.ReturnType == m_type.Module.TypeSystem.Boolean &&
                        m.Parameters.Count <= 2 &&
                        (m.IsStatic || m.IsStatic == prop.SetMethod.IsStatic)
                         );

                        if (condition != null)
                        {
                            MethodDefinition conditionDef = condition.Resolve();
                            if (!conditionDef.IsStatic)
                                instructions.Add(il.Create(OpCodes.Ldarg_0));
                            if (condition.HasGenericParameters)
                                condition = condition.MakeGeneric(new TypeReference[] { prop.PropertyType });
                            switch (condition.Parameters.Count)
                            {
                                case 2:
                                    VariableDefinition local = new VariableDefinition(prop.GetMethod.ReturnType);

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
                            if (prop.SetMethod.IsStatic)
                            {
                                throw new System.MissingMethodException($"Property {prop.DeclaringType.FullName}.{prop.Name}: Could not find method 'static bool {sync.ConditionFunction}()' with 0 to 2 parameters of type {prop.PropertyType.FullName}");
                            } else
                            {
                                throw new System.MissingMethodException($"Property {prop.DeclaringType.FullName}.{prop.Name}: Could not find method 'bool {sync.ConditionFunction}()' or 'static bool {sync.ConditionFunction}()' with 0 to 2 parameters of type {prop.PropertyType.FullName}");
                            }
                        }
                    }

                    // todo: fix
                    instructions.AddRange(new Instruction[]{
                        il.Create(OpCodes.Ldarg_0),
                        il.Create(OpCodes.Ldfld, PendingUpdatesField),
                        il.Create(OpCodes.Ldc_I4, (int)CommandType.Set),
                        il.Create(OpCodes.Ldc_I4, sync.Id),
                        il.Create(OpCodes.Newobj, m_type.Module.ImportReference(typeof(Command<int>).GetConstructor(new []
                        {
                            typeof(CommandType),
                            typeof(int)
                        }))),
                        il.Create(OpCodes.Callvirt, m_type.Module.ImportReference(typeof(List<Command<int>>).GetMethod("Add"))),
                        nop
                    });

                    il.Body.Instructions.AddRangeAt(instructions, 0);
                }
            }
        }

        public void MakeSynced(MethodDefinition method, Authority authority)
        {
            CustomAttribute syncAttribute = method.GetCustomAttribute(typeof(SynchronizeCallAttribute));
            SynchronizeCallAttribute sync;
            if ((sync = syncAttribute.ToAttributeObject<SynchronizeCallAttribute>()) != null && !sync.IsSynchronized)
            {
                syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.IsSynchronized),
                    new CustomAttributeArgument(m_type.Module.TypeSystem.Boolean, true)
                    ));

                if (sync.Authority != authority)
                {
                    SynchronizeObjectAttribute.UIDGenerator.Remove(sync.Id);
                    return;
                }

                ILProcessor il = method.Body.GetILProcessor();

                if (!sync.ExecuteOnAuthority && sync.Authority == authority)
                {
                    il.Body.Instructions.Clear();
                    il.Emit(OpCodes.Ret);
                }

                il.Body.Instructions.AddRangeAt(new Instruction[]{
                        il.Create(OpCodes.Ldarg_0),
                        il.Create(OpCodes.Ldfld, PendingUpdatesField),
                        il.Create(OpCodes.Ldc_I4, (int)CommandType.Invoke),
                        il.Create(OpCodes.Ldc_I4, sync.Id),
                        il.Create(OpCodes.Newobj, m_type.Module.ImportReference(typeof(Command<int>).GetConstructor(new []
                        {
                            typeof(CommandType),
                            typeof(int)
                        }))),
                        il.Create(OpCodes.Callvirt, m_type.Module.ImportReference(typeof(List<Command<int>>).GetMethod("Add"))),
                        il.Create(OpCodes.Nop)
                    }, 0);
            }
        }

        public void RegisterSerializer(MethodDefinition method, SerializationTable table)
        {
            if (method.GetAttribute(typeof(SerializationSpecificationAttribute)) is SerializationSpecificationAttribute spec)
            {
                if (!method.IsStatic) throw new System.Exception($"Serializer method {method} must be declared as static");
                if (!method.HasParameters) throw new System.Exception($"Serializer method {method} must have at least 1 parameter of type {spec.Type}");
                if (spec.Direct && method.Parameters.FirstOrDefault(p => p.ParameterType == MessageType) == null)
                    throw new System.Exception($"Serializer method {method} is declared as Direct so it must have 1 paramter of type {MessageType}");

                table.Serializers.RegisterSerializer(spec.Type, method, spec.Strict);
            }
        }

        public void RegisterSerializers(SerializationTable table)
        {
            foreach (MethodDefinition method in m_type.Methods)
            {
                RegisterSerializer(method, table);
            }
        }

        public void MakeSynced(
            Authority authority,
            bool defaultPropertyAuthority = true,
            bool defaultMethodAuthority = true,
            bool defaultNestedClassAuthority = true)
        {
            PropertyDefinition[] properties = new PropertyDefinition[m_type.Properties.Count];
            m_type.Properties.CopyTo(properties, 0);
            foreach (PropertyDefinition property in properties)
            {
                if (property.CheckAuthority(authority, defaultPropertyAuthority))
                {
                    MakeSynced(property, authority);
                } else
                {
                    property.DeclaringType.Methods.Remove(property.GetMethod);
                    property.DeclaringType.Methods.Remove(property.GetMethod);

                    property.DeclaringType.Properties.Remove(property);
                }
            }
            MethodDefinition[] methods = new MethodDefinition[m_type.Methods.Count];
            m_type.Methods.CopyTo(methods, 0);
            foreach (MethodDefinition method in methods)
            {
                if (method.CheckAuthority(authority, defaultMethodAuthority))
                {
                    MakeSynced(method, authority);
                } else
                {
                    //TypeDefinition declType = method.DeclaringType;
                    method.DeclaringType.Methods.Remove(method);
                    //method.DeclaringType = InjectedModule.Types.Where(t => t.FullName == declType.FullName).FirstOrDefault();
                    //m_type.Module.ImportReference(method);
                }
            }

            TypeDefinition[] types = new TypeDefinition[m_type.NestedTypes.Count];
            m_type.NestedTypes.CopyTo(types, 0);
            foreach (TypeDefinition type in types)
            {
                if (!type.IsClass) continue;
                TypeProcessor tp = new TypeProcessor(type);
                if (!type.CheckAuthority(authority, defaultNestedClassAuthority))
                {
                    m_type.NestedTypes.Remove(type);
                }
                else
                {
                    tp.MakeSynced(authority);
                }
            }

            PropertyDefinition networkUpdateRateProperty = new PropertyDefinition("NetworkUpdateRate", PropertyAttributes.None, m_type.Module.TypeSystem.Single);
            m_type.Properties.Add(networkUpdateRateProperty);

            m_type.Fields.Add(PendingUpdatesField);

            bool isNetworkUpdateRateConstant = false;
            float networkUpdateRate = 0;

            CustomAttribute syncAttribute = m_type.GetCustomAttribute(typeof(SynchronizeClassAttribute));
            SynchronizeClassAttribute sync = syncAttribute.ToAttributeObject<SynchronizeClassAttribute>();
            if (sync != null && !sync.IsSynchronized)
            {
                syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.IsSynchronized),
                    new CustomAttributeArgument(m_type.Module.TypeSystem.Boolean, true)
                    ));

                if (sync.MaxUpdateRate != 0)
                {
                    isNetworkUpdateRateConstant = true;
                    networkUpdateRate = sync.MaxUpdateRate;
                }
            }

            {
                MethodDefinition get_networkUpdateRate = new MethodDefinition(
                    "get_NetworkUpdateRate",
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    m_type.Module.TypeSystem.Single
                    );

                ILProcessor il = get_networkUpdateRate.Body.GetILProcessor();

                if (isNetworkUpdateRateConstant)
                {
                    il.Emit(OpCodes.Ldc_R4, networkUpdateRate);
                    il.Emit(OpCodes.Ret);
                }
                else
                {
                    il.Emit(OpCodes.Ldsfld, NetworkUpdateRateField);
                    il.Emit(OpCodes.Ret);
                }

                m_type.Methods.Add(get_networkUpdateRate);

                networkUpdateRateProperty.GetMethod = get_networkUpdateRate;
            }

            if (!isNetworkUpdateRateConstant)
            {
                m_type.Fields.Add(NetworkUpdateRateField);

                // NetworkUpdateRate.set

                MethodDefinition set_networkUpdateRate = new MethodDefinition(
                    "set_NetworkUpdateRate",
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    m_type.Module.TypeSystem.Void
                    );

                set_networkUpdateRate.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, m_type.Module.TypeSystem.Single));

                ILProcessor il = set_networkUpdateRate.Body.GetILProcessor();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Stsfld, NetworkUpdateRateField);
                il.Emit(OpCodes.Ret);

                m_type.Methods.Add(set_networkUpdateRate);

                networkUpdateRateProperty.SetMethod = set_networkUpdateRate;
            }
        }
    }
}
