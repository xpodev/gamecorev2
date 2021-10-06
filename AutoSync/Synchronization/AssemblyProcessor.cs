using GameCore.Net.Sync.Extensions;
using GameCore.Net.Sync.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GameCore.Net.Sync.Processors
{
    public class AssemblyProcessor : Processor<AssemblyDefinition>
    {
       public AssemblyProcessor(AssemblyDefinition assembly) : base(assembly) { }

        public override bool Process(SynchronizationSettings settings)
        {
            // todo: move this outside
            settings.RPCDispatcher = new RPCDispatcher(new TypeDefinition("<AutoSync>", "RPCDispatcher", TypeAttributes.NotPublic));

            settings.SerializationTable.Clear();

            foreach (CustomAttribute attribute in settings.NetworkManager.CustomAttributes)
            {
                if (attribute.AttributeType.IsEqualTo(Item.MainModule.ImportReference(typeof(ExternalSerializersAttribute))))
                {
                    TypeDefinition type = Item.MainModule.ImportReference(attribute.ConstructorArguments[0].Value as TypeReference).Resolve();
                    new TypeProcessor(type).RegisterSerializers(settings);
                }
            }

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

            {
                Item.MainModule.Types.Add(settings.RPCDispatcher.DeclaringType);
                MethodDefinition dispatcher = settings.RPCDispatcher.GenerateRPCDispathcerType(settings);

                MethodDefinition dispatcherWrapper = new MethodDefinition(
                    "DispatchMessage",
                    MethodAttributes.Static | MethodAttributes.Public,
                    dispatcher.ReturnType
                );

                ParameterDefinition messageParameter = new ParameterDefinition(settings.MessageSettings.MessageType);
                dispatcherWrapper.Parameters.Add(messageParameter);

                ILProcessor il = dispatcherWrapper.Body.GetILProcessor();

                ILGenerator.GenerateLoadArgument(il, messageParameter);
                il.Emit(OpCodes.Call, dispatcher);
                il.Emit(OpCodes.Ret);

                settings.NetworkManager.Methods.Add(dispatcherWrapper);
            }

            return true;
        }
    }
}
