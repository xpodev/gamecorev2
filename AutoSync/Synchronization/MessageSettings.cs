using Mono.Cecil;

namespace GameCore.Net.Sync
{
    public class MessageSettings
    {
        public TypeReference MessageType { get; set; }

        public TypeReference MessageConfigurationType { get; set; }

        public MethodReference MessageConstructor { get; set; }

        public MethodReference MessageSenderMethod { get; set; }

        public MethodReference MessageIDGetter { get; set; }
    }
}
