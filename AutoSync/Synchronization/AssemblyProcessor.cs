using Mono.Cecil;
using GameCore.Net.Sync.Extensions;
using System.Collections.Generic;

namespace GameCore.Net.Sync.Processors
{
    public class AssemblyProcessor : Processor<AssemblyDefinition>
    {
       public AssemblyProcessor(AssemblyDefinition assembly) : base(assembly) { }

        public override bool Process(SynchronizationSettings settings)
        {
            foreach (TypeDefinition type in Item.MainModule.Types.Copy())
            {
                new TypeProcessor(type).RegisterSerializers(settings);
            }

            foreach (TypeDefinition type in Item.MainModule.Types.Copy())
            {
                if (type == settings.MessageSettings.MessageType || type == settings.MessageSettings.MessageSenderMethod.DeclaringType) continue;

                if (!new TypeProcessor(type).Process(settings))
                {
                    Item.MainModule.Types.Remove(type);
                }
            }
            return true;
        }
    }
}
