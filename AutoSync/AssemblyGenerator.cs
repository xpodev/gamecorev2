using Mono.Cecil;
using GameCore.Net.Sync.Extensions;

namespace GameCore.Net.Sync.Generators
{
    public class AssemblyGenerator
    {
        private readonly AssemblyDefinition m_assembly;

        public AssemblyDefinition Assembly => m_assembly;

        public AssemblyGenerator(AssemblyDefinition assembly)
        {
            m_assembly = assembly;
        }

        public AssemblyGenerator MakeSynced(Authority authority, bool defaultSyncBehaviour = false, bool defaultTypeAuthorityCheck = true)
        {
            SerializationTable serializationTable = new SerializationTable();

            foreach (TypeDefinition type in m_assembly.MainModule.Types.Copy())
            {
                TypeProcessor typeGenerator = new TypeProcessor(type);

                if (!type.CheckAuthority(authority, defaultTypeAuthorityCheck)) m_assembly.MainModule.Types.Remove(type);

                typeGenerator.MessageType = m_assembly.MainModule.ImportReference(typeof(Message<int>));

                typeGenerator.RegisterSerializers(serializationTable);

                if (defaultSyncBehaviour || type.HasAttribute(typeof(SynchronizeClassAttribute)))
                {
                    typeGenerator.MakeSynced(authority, defaultNestedClassAuthority: defaultTypeAuthorityCheck);
                }
            }
            return this;
        }
    }
}
