using System;

namespace GameCore.Net.Sync
{
    public class ForwardedMessageTypeAttribute : MessageTypeAttribute
    {
        public Type Type { get; }

        public ForwardedMessageTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
