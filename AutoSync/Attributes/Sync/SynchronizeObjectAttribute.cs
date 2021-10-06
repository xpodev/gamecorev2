using System;

namespace GameCore.Net.Sync
{
    using UIDType = Int32;

    class DefaultUIDGenerator : IUIDGenerator<UIDType>
    {
        private UIDType m_uid = 0;

        public UIDType GenerateUID()
        {
            return m_uid++;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public abstract class SynchronizeObjectAttribute : SynchronizeAttribute
    {
        public UIDType Id { get; set; }

        public Authority Authority { get; }

        public SynchronizeObjectAttribute(Authority authority)
        {
            Authority = authority;
        }
    }
}
