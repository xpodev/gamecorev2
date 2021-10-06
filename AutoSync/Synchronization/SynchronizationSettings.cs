using Mono.Cecil;

namespace GameCore.Net.Sync
{
    public class SynchronizationSettings
    {
        public MessageSettings MessageSettings { get; set; }

        public TypeDefinition ObjectIdType { get; set; }

        public TypeDefinition NetworkManager { get; set; }

        public RPCDispatcher RPCDispatcher { get; set; }

        public SerializationTable SerializationTable { get; set; }

        public Authority Authority { get; set; }

        public bool IncludeNonAuthorityClasses { get; set; }

        public bool IncludeNonAuthorityProperties { get; set; }

        public bool IncludeNonAuthorityMethods { get; set; }

        public bool IncludeNonAuthorityNestedClasses { get; set; }

        public bool CreateSharedLibrary { get; set; }

        public string SharedLibraryPath { get; set; }

        public string ClientLibraryPath { get; set; }

        public string ServerLibraryPath { get; set; }
    }
}
