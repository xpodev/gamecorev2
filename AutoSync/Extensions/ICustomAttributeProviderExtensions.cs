using System;
using Mono.Cecil;

namespace GameCore.Net.Sync.Extensions
{
    public static class ICustomAttributeProviderExtensions
    {
        public static CustomAttribute GetCustomAttribute<U>(this U member, Type type)
            where U : MemberReference, ICustomAttributeProvider
        {
            TypeDefinition attrDefinition = member.Module.ImportReference(type).Resolve();
            foreach (CustomAttribute attribute in member.CustomAttributes)
            {
                if (attribute.AttributeType.Resolve().IsClassAssignableFrom(attrDefinition))
                {
                    return attribute;
                }
            }
            return null;
        }

        public static object GetAttribute<U>(this U member, Type type)
            where U : MemberReference, ICustomAttributeProvider
        {
            return member.GetCustomAttribute(type).ToAttributeObject(type);
        }

        public static bool HasAttribute<U>(this U member, Type type)
            where U : MemberReference, ICustomAttributeProvider
        {
            return member.GetAttribute(type) != null;
        }

        public static bool CheckAuthority<U>(this U o, Authority authority, bool defaultValue = true)
            where U : MemberReference, ICustomAttributeProvider
        {
            if (o.GetAttribute(typeof(AuthorityCodeAttribute)) is AuthorityCodeAttribute authorityCode)
            {
                return authorityCode.Authority == authority;
            }
            return defaultValue;
        }
    }
}
