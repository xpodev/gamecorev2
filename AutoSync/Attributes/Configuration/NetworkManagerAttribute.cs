using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NetworkManagerAttribute: Attribute
    {
        public string MessageSenderName { get; set; }

        public NetworkManagerAttribute(Type messageType)
        {
            
        }
    }
}
