using Mono.Cecil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Processors
{
    public static class AssemblyPreProcessor
    {
        static DefaultUIDGenerator
            ClientUIDGenerator = new DefaultUIDGenerator(),
            ServerUIDGenerator = new DefaultUIDGenerator();

        public static void ProcessMember<T, U>(T member)
            where T : MemberReference, ICustomAttributeProvider
            where U : SynchronizeObjectAttribute
        {
            if (member.GetCustomAttribute(typeof(U)) is CustomAttribute syncAttribute && syncAttribute.ToAttributeObject(typeof(U)) is U sync)
            {
                var uid = (sync.Authority == Authority.Client ? ClientUIDGenerator : ServerUIDGenerator).GenerateUID();
                syncAttribute.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.Id),
                    new CustomAttributeArgument(
                        member.Module.TypeSystem.Int32,
                        uid
                    )
                ));
                System.Console.WriteLine($"[{sync.Authority}] {member.Name} => {uid}");
            }
        }

        public static void ProcessType(TypeDefinition type)
        {
            foreach (PropertyDefinition property in type.Properties)
            {
                ProcessMember<PropertyDefinition, SynchronizeValueAttribute>(property);
            }
            foreach (MethodDefinition method in type.Methods)
            {
                ProcessMember<MethodDefinition, SynchronizeCallAttribute>(method);
            }
        }

        public static void ProcessAssembly(AssemblyDefinition assembly)
        {
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                ProcessType(type);
            }
        }
    }
}
