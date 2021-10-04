using Mono.Cecil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Generators
{
    public static class AssemblyProcessor
    {

        public static void ProcessProperty(PropertyDefinition property)
        {
            CustomAttribute syncValue = property.GetCustomAttribute(typeof(SynchronizeValueAttribute));
            SynchronizeValueAttribute sync;
            if ((sync = syncValue.ToAttributeObject<SynchronizeValueAttribute>()) != null)
            {
                System.Console.WriteLine($"{property.Name} => {sync.Id}");
                syncValue.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.Id), new CustomAttributeArgument(property.Module.TypeSystem.Int32, sync.Id)));
            }
        }

        public static void ProcessMethod(MethodDefinition method)
        {
            CustomAttribute syncValue = method.GetCustomAttribute(typeof(SynchronizeCallAttribute));
            SynchronizeCallAttribute sync;
            if ((sync = syncValue.ToAttributeObject<SynchronizeCallAttribute>()) != null)
            {
                System.Console.WriteLine($"{method.Name} => {sync.Id}");
                syncValue.Properties.Add(new CustomAttributeNamedArgument(
                    nameof(sync.Id), new CustomAttributeArgument(method.Module.TypeSystem.Int32, sync.Id)));
            }
        }

        public static void ProcessType(TypeDefinition type)
        {
            foreach (PropertyDefinition property in type.Properties)
            {
                ProcessProperty(property);
            }
            foreach (MethodDefinition method in type.Methods)
            {
                ProcessMethod(method);
            }
        }

        public static void ProcessAssembly(AssemblyDefinition assembly)
        {
            NetworkConfigAttribute config;

            if ((config = assembly.GetAttribute<NetworkConfigAttribute>()) != null)
            {
                
            }

            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                ProcessType(type);
            }
        }
    }
}
