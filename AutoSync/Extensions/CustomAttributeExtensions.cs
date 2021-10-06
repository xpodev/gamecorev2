using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class CustomAttributeExtensions
    {
        public static (Type[], object[]) BuildConstructorData(IList<CustomAttributeArgument> args)
        {
            Type[] types = new Type[args.Count];
            object[] values = new object[args.Count];
            foreach ((int i, CustomAttributeArgument arg) in new Enumerate<CustomAttributeArgument>(args))
            {
                if (arg.Value is TypeReference tRef)
                {
                    types[i] = Type.GetType(arg.Type.FullName);
                    values[i] = Type.GetType(tRef.AssemblyQualifiedName());
                }
                else if (arg.Value is CustomAttributeArgument cVal)
                {
                    types[i] = cVal.Value?.GetType();
                    values[i] = cVal.Value;
                }
                else if (arg.Value is CustomAttributeArgument[] cVals)
                {
                    (Type[] _, object[] vs) = BuildConstructorData(cVals);
                    Type type = Type.GetType(arg.Type.FullName).GetElementType();
                    types[i] = type.MakeArrayType();
                    Type listType = typeof(List<>).MakeGenericType(type);
                    var listAdd = listType.GetMethod("Add", new Type[] { type });
                    object arr = Activator.CreateInstance(listType, vs.Length);
                    object[] listAddArgs = new object[1];
                    foreach (object value in vs)
                    {
                        listAddArgs[0] = Convert.ChangeType(value, type);
                        listAdd.Invoke(arr, listAddArgs);
                    }
                    values[i] = listType.GetMethod("ToArray").Invoke(arr, null);
                } 
                else
                {
                    types[i] = arg.Value.GetType();
                    values[i] = arg.Value;
                }
            }
            return (types, values);
        }

        public static object ToAttributeObject(this CustomAttribute attribute, Type type = null)
        {
            if (attribute == null) return null;
            type = Type.GetType(attribute.AttributeType.AssemblyQualifiedName()) ?? type;
            (Type[] types, object[] args) = BuildConstructorData(attribute.ConstructorArguments);
            var cons = type.GetConstructor(types);
            object o = cons.Invoke(args);

            foreach (CustomAttributeNamedArgument field in attribute.Fields)
            {
                type.GetField(field.Name).SetValue(o, field.Argument.Value);
            }

            foreach (CustomAttributeNamedArgument property in attribute.Properties)
            {
                System.Reflection.PropertyInfo prop = type.GetProperty(property.Name, 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    );
                if (prop?.SetMethod != null) prop.SetValue(o, property.Argument.Value);
            }

            return o;
        }

        public static T ToAttributeObject<T>(this CustomAttribute attribute) where T : Attribute
        {
            return attribute.ToAttributeObject(typeof(T)) as T;
        }
    }
}
