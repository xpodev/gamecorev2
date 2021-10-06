using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Processors
{
    public class TypeProcessor : Processor<TypeDefinition>
    {
        public FieldDefinition NetworkUpdateRateField { get; }

        public TypeProcessor(TypeDefinition type) : base(type)
        {
            const string NetworkUpdateRateFieldName = "<NetworkUpdateRate>k__generatedField";

            if ((NetworkUpdateRateField = Item.Fields.Where(field =>
                field.Name == NetworkUpdateRateFieldName &&
                field.FieldType == Item.Module.TypeSystem.Single).FirstOrDefault()
            ) == null)
            {
                NetworkUpdateRateField = new FieldDefinition(
                NetworkUpdateRateFieldName,
                FieldAttributes.Private,
                Item.Module.TypeSystem.Single
                );
            }
        }

        public void RegisterSerializer(MethodReference method, SynchronizationSettings settings)
        {
            MethodDefinition definition = method.Resolve();

            if (!definition.CheckAuthority(settings.Authority, settings.IncludeNonAuthorityMethods))
            {
                return;
            }

            if (definition.GetAttribute(typeof(SerializationSpecificationAttribute)) is SerializationSpecificationAttribute spec)
            {
                if (!spec.Direct && !definition.IsStatic) throw new System.Exception($"Serializer method {method} must be declared as static");
                if (!method.HasParameters) throw new System.Exception($"Serializer method {method} must have at least 1 parameter of type {spec.Type}");
                if (spec.Direct && !method.HasThis && method.Parameters.FirstOrDefault(p => settings.MessageSettings.MessageType.IsClassAssignableFrom(p.ParameterType.Resolve())) == null)
                    throw new System.Exception($"Serializer method {method} is declared as Direct so it must have 1 paramter of type {settings.MessageSettings.MessageType}");

                (spec.Operation == SerializationOperation.Serialize ?settings.Serializers : settings.Deserializers)
                    .RegisterSerializer(Item.Module.ImportReference(spec.Type), method, spec.Strict);
            }
        }

        public void RegisterSerializers(SynchronizationSettings settings)
        {
            if (!Item.CheckAuthority(settings.Authority, Item.DeclaringType == null ? settings.IncludeNonAuthorityClasses : settings.IncludeNonAuthorityNestedClasses))
                return;

            TypeReference type = Item;

            do
            {
                foreach (MethodDefinition method in type.Resolve().Methods)
                {
                    RegisterSerializer(method.MakeGeneric(type), settings);
                }
                // todo: fix generic hierarchy traversal
            } while ((type = type.IsGenericInstance ? null : type.Resolve().BaseType) != null);
        }

        public override bool Process(SynchronizationSettings settings)
        {
            if (!Item.CheckAuthority(settings.Authority, Item.DeclaringType == null ? settings.IncludeNonAuthorityMethods : settings.IncludeNonAuthorityNestedClasses))
            {
                return false;
            }

            if (Item.GetAttribute(typeof(SynchronizeClassAttribute)) is SynchronizeClassAttribute sync)
            {
                PropertyDefinition[] properties = new PropertyDefinition[Item.Properties.Count];
                Item.Properties.CopyTo(properties, 0);
                foreach (PropertyDefinition property in properties)
                {
                    if (!new PropertyProcessor(property).Process(settings))
                    {
                        property.DeclaringType.Methods.Remove(property.GetMethod);
                        property.DeclaringType.Methods.Remove(property.GetMethod);

                        property.DeclaringType.Properties.Remove(property);
                    }
                }

                MethodDefinition[] methods = new MethodDefinition[Item.Methods.Count];
                Item.Methods.CopyTo(methods, 0);
                foreach (MethodDefinition method in methods)
                {
                    if (!new MethodProcessor(method).Process(settings))
                    {
                        method.DeclaringType.Methods.Remove(method);
                    }
                }

                TypeDefinition[] types = new TypeDefinition[Item.NestedTypes.Count];
                Item.NestedTypes.CopyTo(types, 0);
                foreach (TypeDefinition type in types)
                {
                    if (!type.IsClass) continue;
                    if (!new TypeProcessor(type).Process(settings))
                    {
                        Item.NestedTypes.Remove(type);
                    }
                }

                PropertyDefinition networkUpdateRateProperty = new PropertyDefinition("NetworkUpdateRate", PropertyAttributes.None, Item.Module.TypeSystem.Single);
                Item.Properties.Add(networkUpdateRateProperty);

                //Item.Fields.Add(PendingUpdatesField);

                bool isNetworkUpdateRateConstant = false;
                float networkUpdateRate = 0;

                CustomAttribute syncAttribute = Item.GetCustomAttribute(typeof(SynchronizeClassAttribute));
                if (!sync.IsSynchronized)
                {
                    syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                        nameof(sync.IsSynchronized),
                        new CustomAttributeArgument(Item.Module.TypeSystem.Boolean, true)
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
                        Item.Module.TypeSystem.Single
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

                    Item.Methods.Add(get_networkUpdateRate);

                    networkUpdateRateProperty.GetMethod = get_networkUpdateRate;
                }

                if (!isNetworkUpdateRateConstant)
                {
                    Item.Fields.Add(NetworkUpdateRateField);

                    // NetworkUpdateRate.set

                    MethodDefinition set_networkUpdateRate = new MethodDefinition(
                        "set_NetworkUpdateRate",
                        MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                        Item.Module.TypeSystem.Void
                        );

                    set_networkUpdateRate.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Item.Module.TypeSystem.Single));

                    ILProcessor il = set_networkUpdateRate.Body.GetILProcessor();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Stsfld, NetworkUpdateRateField);
                    il.Emit(OpCodes.Ret);

                    Item.Methods.Add(set_networkUpdateRate);

                    networkUpdateRateProperty.SetMethod = set_networkUpdateRate;
                }

                return true;
            }
            return false;
        }
    }
}
