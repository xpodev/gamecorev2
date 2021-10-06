using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SynchronizeCallAttribute : SynchronizeObjectAttribute
    {
        public bool Multicast { get; set; }

        public bool Reliable { get; set; }

        public float Priority { get; set; }

        public bool ExecuteOnAuthority { get; }

        public SynchronizeCallAttribute(Authority authority, bool executeOnAuthority = false) : base(authority)
        {
            ExecuteOnAuthority = executeOnAuthority;
        }
    }
}
