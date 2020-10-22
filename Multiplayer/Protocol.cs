using System;
using System.Collections.Generic;


namespace GameCore.Multiplayer
{
    public class Protocol
    {
        public static Protocol CurrentProtocol
        {
            get;
            private set;
        }

        public static Version Version = new Version(1, 0, 0);

        private class TypeCallbackInfo
        {
            public int id;
            public Action<object> action;
        }

        private readonly Dictionary<Type, TypeCallbackInfo> typesIds = new Dictionary<Type, TypeCallbackInfo>();

        private readonly List<Type> types = new List<Type>();

        public IClock Clock
        {
            get;
            private set;
        }

        private Protocol(IClock clock)
        {
            Clock = clock;
        }

        public static Protocol Instantiate(IClock clock)
        {
            if (CurrentProtocol is null)
            {
                CurrentProtocol = new Protocol(clock);
            }
            return CurrentProtocol;
        }

        public bool ValidateProtocolSettings(ProtocolSettings settings)
        {
            if (settings.Version != Version)
            {
                return false;
            }
            if (settings.RegisteredTypes.Length != types.Count)
            {
                return false;
            }
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] != settings.RegisteredTypes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public Type[] GetResiteredTypes()
        {
            return types.ToArray();
        }

        public int RegisterType(Type t)
        {
            int typeId = typesIds.Count;
            if (typesIds.ContainsKey(t))
            {
                return -1;
            }
            typesIds.Add(t, new TypeCallbackInfo() { id = typeId, action = null });
            types.Add(t);
            return typeId;
        }

        public int RegisterType<T>()
        {
            return RegisterType(typeof(T));
        }

        public void SetCallback(Type t, Action<object> action)
        {
            typesIds[t].action = action;
        }

        public void SetCallback<T>(Action<object> action)
        {
            SetCallback(typeof(T), action);
        }

        public void UnsetCallback(Type t)
        {
            typesIds[t].action = null;
        }

        public void UnsetCallback<T>()
        {
            UnsetCallback(typeof(T));
        }

        public void HandleMessage(Message msg)
        {
            int typeId = msg.TypeId;
            // TODO: encapsulate this in a try block and throwing a custom exception
            typesIds[types[typeId]].action.Invoke(msg.Object);
        }

        public Message CreateMessage(Type t, object obj, long serial)
        {
            return new Message(typesIds[t].id, obj, serial);
        }

        public Message CreateMessage<T>(T obj, long serial)
        {
            return CreateMessage(typeof(T), obj, serial);
        }

        public Message CreateCustomMessage(object obj, long serial, int id = -1)
        {
            return new Message(id, obj, serial);
        }
    }
}
