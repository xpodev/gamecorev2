using System;
using System.Collections.Generic;
using GameCore.Net.Sync;
using RiptideNetworking;

namespace GameCore.Riptide
{
    [ForwardedMessageType(typeof(Message), ConstructorName = "CreateMessage", IdPropertyName = "GetId")]
    public static class MessageConfiguration
    {
        [CustomFunctionCall("Id", "IsReliable")]
        public static Message CreateMessage(ushort id, bool isReliable)
        {
            return Message.Create(isReliable ? MessageSendMode.reliable : MessageSendMode.unreliable, id);
        }

        public static ushort GetId(this Message message)
        {
            return message.GetUShort();
        }

        [TypeSerializer(typeof(string), Direct = true, Strict = true)]
        public static Message InsertString(this Message msg, string s)
        {
            return msg.Add(s);
        }

        [TypeDeserializer(typeof(string), Direct = true, Strict = true)]
        public static string ExtractString(this Message msg)
        {
            return msg.GetString();
        }
    }
}
