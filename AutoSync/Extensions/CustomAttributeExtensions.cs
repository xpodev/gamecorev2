using System;
using System.Linq;
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
                    values[i] = Type.GetType(tRef.FullName);
                }
                else if (arg.Value is CustomAttributeArgument cVal)
                {
                    types[i] = cVal.Value?.GetType();
                    values[i] = cVal.Value;
                }
                else if (arg.Value is CustomAttributeArgument[] cVals)
                {
                    (Type[] _, object[] vs) = BuildConstructorData(cVals);
                    types[i] = Type.GetType(arg.Type.FullName).MakeArrayType();
                    values[i] = vs;
                } else
                {
                    types[i] = arg.Value.GetType();
                    values[i] = arg.Value;
                }
            }
            return (types, values);
        }

        public static object ToAttributeObject(this CustomAttribute attribute, System.Type type = null)
        {
            if (attribute == null) return null;
            type = Type.GetType(attribute.AttributeType.FullName) ?? type;
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
                if (prop.SetMethod != null) prop.SetValue(o, property.Argument.Value);
            }

            return o;
        }

        public static T ToAttributeObject<T>(this CustomAttribute attribute) where T : System.Attribute
        {
            return attribute.ToAttributeObject(typeof(T)) as T;
        }
    }
}
