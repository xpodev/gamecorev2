using System;
using System.Linq;
using System.Collections.Generic;
using GameCore.Net.Sync.Extensions;
using Mono.Cecil;

namespace GameCore.Net.Sync
{
    public class SerializationTable
    {
        public class SerializersTable
        {
            private readonly Dictionary<Type, MethodReference> m_strictSerializers = new Dictionary<Type, MethodReference>();
            private readonly Dictionary<Type, MethodReference> m_nonStrictSerializers = new Dictionary<Type, MethodReference>();

            public void RegisterSerializer(Type type, MethodDefinition serializer, bool strict)
            {
                if (!serializer.IsStatic)
                {
                    throw new Exception($"Serializer method must be static");
                }

                MethodReference method = serializer;

                if (method.ReturnType.IsGenericParameter)
                {
                    throw new Exception($"Serializer method return type can't be generic");
                }

                ParameterDefinition objParameter;
                if (serializer.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute call) {
                    int objIndex = Array.IndexOf(call.Args, "Object");
                    if (objIndex == -1)
                    {
                        throw new Exception("Serializer custom call must include the 'Object' parameter");
                    }
                    objParameter = serializer.Parameters[objIndex];
                    if (serializer.Parameters.Count != call.Args.Length)
                    {
                        throw new Exception($"Serializer {serializer} has custom call, but the amount of arguments is not the same as the amount of parameters");
                    }
                } else
                {
                    if (serializer.Parameters.Count != 1)
                        throw new Exception($"Serializer {serializer} must have exactly 1 parameter");
                    objParameter = serializer.Parameters[0];
                }

                if (strict)
                {
                    if (method.HasGenericParameters && objParameter.ParameterType.IsGenericParameter)
                    {
                        method = method.MakeGeneric(method.Module.ImportReference(type));
                    }
                    if (method.HasGenericParameters) throw new Exception($"Only the object parameter can be generic");
                    if (m_strictSerializers.ContainsKey(type))
                    {
                        throw new Exception($"There's already a serializer registered for type {type}");
                    }
                    TypeReference tref = serializer.Module.ImportReference(type);
                    if (!method.IsGenericInstance && objParameter.ParameterType.Resolve() != tref.Resolve())
                        throw new Exception($"{serializer} first (or Object) argument must be of type {type}");
                    m_strictSerializers.Add(type, method);
                }
                else
                {
                    if (!objParameter.ParameterType.IsGenericParameter && 
                        !serializer.Module.ImportReference(type).Resolve().IsClassAssignableFrom(objParameter.ParameterType.Resolve()))
                    {
                        throw new Exception($"Type {type} can't be assigned to {objParameter.ParameterType}");
                    }
                    if (m_nonStrictSerializers.ContainsKey(type))
                    {
                        throw new Exception($"There's already a serializer registered for type {type}");
                    }
                    m_nonStrictSerializers.Add(type, method);
                }
            }

            public MethodReference GetSerializer(Type type, bool strict)
            {
                MethodReference result;
                if (strict)
                {
                    if (m_strictSerializers.TryGetValue(type, out result))
                    {
                        return result;
                    }
                    return null;
                }
                else
                {
                    do
                    {
                        if (m_nonStrictSerializers.TryGetValue(type, out result))
                        {
                            if (result.HasGenericParameters)
                            {
                                return result.MakeGeneric(result.Module.ImportReference(type));
                            }
                            return result;
                        }
                    } while ((type = type.BaseType) != null);
                    return null;
                }
            }

            public MethodReference GetSerializer(Type type)
            {
                return GetSerializer(type, true) ?? GetSerializer(type, false);
            }
        }

        private readonly SerializersTable m_serializers = new SerializersTable();

        public SerializersTable Serializers => m_serializers;
    }
}
