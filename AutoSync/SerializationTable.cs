using System;
using System.Collections.Generic;
using GameCore.Net.Sync.Extensions;
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

        private readonly Dictionary<TypeWrapper, MethodReference> m_strictSerializers = new Dictionary<TypeWrapper, MethodReference>();
        private readonly Dictionary<TypeWrapper, MethodReference> m_nonStrictSerializers = new Dictionary<TypeWrapper, MethodReference>();

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
                if (serializer.Parameters.Count != 1)
                    throw new Exception($"Serializer {serializer} must have exactly 1 parameter");
                objParameter = serializer.Parameters[0];
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
                m_strictSerializers.Add(wrapper, method);
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
                m_nonStrictSerializers.Add(wrapper, method);
            }
        }

        public MethodReference GetNonStrictSerializer(TypeDefinition type)
        {
            MethodReference result;
            do
            {
                if (m_nonStrictSerializers.TryGetValue(new TypeWrapper(type), out result))
                {
                    if (result.HasGenericParameters)
                    {
                        return result.MakeGeneric(result.Module.ImportReference(type));
                    }
                    return result;
                }
            } while ((type = type.BaseType?.Resolve()) != null);
            return null;
        } 

        public MethodReference GetSerializer(TypeReference type, bool strict)
        {
            MethodReference result;
            if (strict)
            {
                if (m_strictSerializers.TryGetValue(new TypeWrapper(type), out result))
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

        public MethodReference GetSerializer(TypeReference type)
        {
            return GetSerializer(type, true) ?? GetSerializer(type, false);
        }
    }
}
