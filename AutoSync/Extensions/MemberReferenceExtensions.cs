using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class MemberReferenceExtensions
    {
        public static MemberReference RelativeTo(this MemberReference member, ModuleDefinition module)
        {
            if (member.DeclaringType == null)
            {
                return module.GetType(member.FullName);
            }

            return member.RelativeTo(module.GetType(member.DeclaringType.FullName));
        }

        public static MemberReference RelativeTo(this MemberReference member, TypeDefinition type)
        {
            //TypeDefinition type = member.DeclaringType.RelativeTo(parent) as TypeDefinition;

            //if (member is TypeDefinition t) return type;

            //if (type == null)
            //{
            //    throw new ArgumentException();
            //}

            if (member is MethodReference m) return type.GetMethod(m.Name, m.Parameters.Select(p_ => p_.ParameterType).ToArray());
            if (member is PropertyDefinition p) return type.GetProperty(p.Name, p.PropertyType);
            if (member is FieldDefinition f) return type.GetField(f.Name, f.FieldType);

            throw new ArgumentException();
        }
    }
}
