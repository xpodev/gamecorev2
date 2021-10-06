using Mono.Cecil;

namespace GameCore.Net.Sync
{
    public class MessageSettings
    {
        public TypeDefinition MessageType { get; set; }

        public MethodDefinition MessageConstructor { get; set; }

        public MethodDefinition MessageSenderMethod { get; set; }

        public MethodReference MessageIDGetter { get; set; }
    }
}
