using System;
using System.Collections.Generic;


namespace GameCore.Multiplayer
{
    public class Protocol<T> where T : Enum
    {
        public delegate void MessageHandler(Message<T> msg);

        public static Protocol<T> CurrentProtocol
        {
            get;
            private set;
        }

        public static Version Version = new Version(1, 0, 0);

        private MessageHandler[] handlers;

        public MessageHandler DefaultHandler
        {
            get;
            set;
        }

        public IClock Clock
        {
            get;
            private set;
        }

        private Protocol(IClock clock)
        {
            Clock = clock;
            handlers = new MessageHandler[Enum.GetValues(typeof(T)).Length];
        }

        public static Protocol<T> Instantiate(IClock clock)
        {
            if (CurrentProtocol is null)
            {
                CurrentProtocol = new Protocol<T>(clock);
            }
            return CurrentProtocol;
        }

        public void SetHandler(int commandId, MessageHandler handler)
        {
            if (commandId >= handlers.Length || commandId < 0)
            {
                throw new ArgumentOutOfRangeException($"Tried to register a command that doesn't exist (Command Id: {commandId})");
            }
            handlers[commandId] = handler;
        }

        public void SetHandler(T commandId, MessageHandler handler)
        {
            SetHandler(Convert.ToInt32(commandId), handler);
        }

        public void UnSetHandler(int commandId)
        {
            if (commandId >= handlers.Length || commandId < 0)
            {
                throw new ArgumentOutOfRangeException($"Tried to unregister a command that doesn't exist (Command Id: {commandId})");
            }
            handlers[commandId] = null;
        }

        public void UnSetHandler(T commandId)
        {
            UnSetHandler(Convert.ToInt32(commandId));
        }

        public bool ValidateProtocolSettings(ProtocolSettings<T> settings)
        {
            if (settings.Version != Version)
            {
                return false;
            }
            if (Enum.GetUnderlyingType(settings.EnumType) != Enum.GetUnderlyingType(typeof(T)))
            {
                return false;
            }
            return true;
        }

        public void HandleMessage(Message<T> msg)
        {
            int handlerIndex = Convert.ToInt32(msg.CommandId);
            MessageHandler handler = handlers[handlerIndex];
            if (handler is null)
            {
                if (DefaultHandler is null)
                {
                    throw new ArgumentNullException($"Handler for command (Id: {handlerIndex}) is not set");
                }
                DefaultHandler.Invoke(msg);
            }
            else
            {
                handler.Invoke(msg);
            }
        }

        public Message<T> CreateMessage(T command, object obj, long serial)
        {
            return new Message<T>(command, obj, serial);
        }
    }
}
