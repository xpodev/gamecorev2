using System;
using System.Collections.Generic;
using GameCore.Net.Sync.Extensions;
using GameCore.Net.Sync.Internal;
using Mono.Cecil;

namespace GameCore.Net.Sync
{
    public class SerializationTable
    {
        private struct TypeWrapper
        {
            private TypeReference m_type;

            public TypeWrapper(TypeReference type)
            {
                m_type = type;
            }

            public override bool Equals(object obj)
            {
                if (obj is TypeReference otherType)
                {
                    return m_type.IsEqualTo(otherType);
                } else if (obj is TypeWrapper otherWrapper)
                {
                    return m_type.IsEqualTo(otherWrapper.m_type);
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return m_type.FullName.GetHashCode();
            }
        }

        private readonly Dictionary<TypeWrapper, SerializationMethodWrapper> m_strictSerializers = new Dictionary<TypeWrapper, SerializationMethodWrapper>();
        private readonly Dictionary<TypeWrapper, SerializationMethodWrapper> m_nonStrictSerializers = new Dictionary<TypeWrapper, SerializationMethodWrapper>();

        private readonly Dictionary<TypeWrapper, SerializationMethodWrapper> m_strictDeserializers = new Dictionary<TypeWrapper, SerializationMethodWrapper>();
        private readonly Dictionary<TypeWrapper, SerializationMethodWrapper> m_nonStrictDeserializers = new Dictionary<TypeWrapper, SerializationMethodWrapper>();

        public void RegisterSerializer(TypeReference type, MethodReference method, bool strict)
        {
            //if (!serializer.IsStatic)
            //{
            //    throw new Exception($"Serializer method must be static");
            //}

            MethodDefinition serializer = method.Resolve();

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
            } 
            else
            {
                if (serializer.IsStatic && serializer.Parameters.Count == 2)
                {
                    objParameter = serializer.Parameters[1];
                }
                else if (serializer.Parameters.Count != 1)
                    throw new Exception($"Serializer {serializer} must have exactly 1 parameter");
                else objParameter = serializer.Parameters[0];
            }

            TypeWrapper wrapper = new TypeWrapper(type);

            if (strict)
            {
                if (method.HasGenericParameters && objParameter.ParameterType.IsGenericParameter)
                {
                    method = method.MakeGeneric(method.Module.ImportReference(type));
                }
                if (method.HasGenericParameters) throw new Exception($"Only the object parameter can be generic");
                if (m_strictSerializers.ContainsKey(wrapper))
                {
                    throw new Exception($"There's already a serializer registered for type {type}");
                }
                if (!method.IsGenericInstance && !objParameter.ParameterType.IsEqualTo(type))
                    throw new Exception($"{serializer} first (or Object) argument must be of type {type}");
                m_strictSerializers.Add(wrapper, new SerializationMethodWrapper(method));
            }
            else
            {
                if (!objParameter.ParameterType.IsGenericParameter && 
                    !serializer.Module.ImportReference(type).Resolve().IsClassAssignableFrom(objParameter.ParameterType.Resolve()))
                {
                    throw new Exception($"Type {type} can't be assigned to {objParameter.ParameterType}");
                }
                if (m_nonStrictSerializers.ContainsKey(wrapper))
                {
                    throw new Exception($"There's already a serializer registered for type {type}");
                }
                m_nonStrictSerializers.Add(wrapper, new SerializationMethodWrapper(method));
            }
        }

        public void RegisterDeserializer(TypeReference type, MethodReference method, SynchronizationSettings settings)
        {
            if (method.HasGenericParameters)
                throw new Exception($"Deserializers can't have generic parameters");

            SerializationMethodWrapper wrapper = new SerializationMethodWrapper(settings.NetworkManager.Module.ImportReference(method));

            if (wrapper.HasCustomCall)
            {
                Dictionary<string, TypeReference> types = new Dictionary<string, TypeReference>()
                {
                    { "Message", settings.MessageSettings.MessageType },
                    { "Result", new ByReferenceType(type) }
                };
                wrapper.CheckCustomCall(types, wrapper.IsStrict);
            } 
            else
            {
                if (wrapper.IsDirect)
                {
                    if (method.Parameters.Count == 0 && method.HasThis || method.Parameters.Count == 1 && wrapper.IsStatic)
                    {
                        if (!method.ReturnType.IsEqualTo(type))
                            throw new Exception($"{method.FullName} is declared as a deserializer for {type.FullName} but it returns another type");
                    } 
                    else if (method.Parameters.Count == 1 && wrapper.IsStatic)
                    {
                        if (!method.Parameters[0].ParameterType.IsEqualTo(settings.MessageSettings.MessageType))
                            throw new Exception($"Direct deserializers must have exactly 1 parameter of type {settings.MessageSettings.MessageType}. {method.FullName} is not a valid deserializer.");
                    }
                    else if (method.Parameters.Count == 2)
                    {
                        if (!wrapper.CheckCall(new TypeReference[]
                        {
                            settings.MessageSettings.MessageType,
                            method.Parameters[1].ParameterType
                        }, wrapper.IsStrict))
                            throw new Exception($"");
                    }
                    else throw new Exception($"{method.FullName} is declared as a deserializer but it has an invalid amount of parameters ({method.Parameters.Count})");
                } 
                else
                {
                    if (method.Parameters.Count != 1)
                        throw new Exception($"Indirect deserializers must have exactly 1 parameter. {method.FullName} is not a valid deserializer.");
                    
                    if (!method.ReturnType.IsEqualTo(type))
                        throw new Exception($"{method.FullName} is declared as a deserializer for {type.FullName} but it returns a another type");
                }
            }

            (wrapper.IsStrict ? m_strictDeserializers : m_nonStrictDeserializers).Add(new TypeWrapper(type), wrapper);
        }

        public void Clear()
        {
            m_strictSerializers.Clear();
            m_strictDeserializers.Clear();
            m_nonStrictSerializers.Clear();
            m_nonStrictDeserializers.Clear();
        }

        internal SerializationMethodWrapper GetNonStrictSerializer(TypeDefinition type)
        {
            do
            {
                if (m_nonStrictSerializers.TryGetValue(new TypeWrapper(type), out SerializationMethodWrapper result))
                {
                    if (result.MethodReference.HasGenericParameters)
                    {
                        return new SerializationMethodWrapper(result.MethodReference.MakeGeneric(result.MethodReference.Module.ImportReference(type)));
                    }
                    return result;
                }
            } while ((type = type.BaseType?.Resolve()) != null);
            return null;
        }

        internal SerializationMethodWrapper GetSerializer(TypeReference type, bool strict)
        {
            if (strict)
            {
                if (m_strictSerializers.TryGetValue(new TypeWrapper(type), out SerializationMethodWrapper result))
                {
                    return result;
                }
                return null;
            }
            else
            {
                return GetNonStrictSerializer(type.Resolve());
            }
        }

        internal SerializationMethodWrapper GetSerializer(TypeReference type)
        {
            return GetSerializer(type, true) ?? GetSerializer(type, false);
        }

        internal SerializationMethodWrapper GetNonStrictDeserializer(TypeDefinition type)
        {
            do
            {
                if (m_nonStrictDeserializers.TryGetValue(new TypeWrapper(type), out SerializationMethodWrapper result))
                {
                    if (result.MethodReference.HasGenericParameters)
                    {
                        return new SerializationMethodWrapper(result.MethodReference.MakeGeneric(result.MethodReference.Module.ImportReference(type)));
                    }
                    return result;
                }
            } while ((type = type.BaseType?.Resolve()) != null);
            return null;
        } 

        internal SerializationMethodWrapper GetDeserializer(TypeReference type, bool strict)
        {
            if (strict)
            {
                if (m_strictDeserializers.TryGetValue(new TypeWrapper(type), out SerializationMethodWrapper result))
                {
                    return result;
                }
                return null;
            }
            else
            {
                return GetNonStrictDeserializer(type.Resolve());
            }
        }

        internal SerializationMethodWrapper GetDeserializer(TypeReference type)
        {
            return GetDeserializer(type, true) ?? GetDeserializer(type, false);
        }
    }
}
