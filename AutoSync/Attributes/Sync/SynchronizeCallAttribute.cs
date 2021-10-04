using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SynchronizeCallAttribute : SynchronizeObjectAttribute
    {
        public bool Multicast { get; set; }

        public float Priority { get; set; }

        public Authority Authority { get; }

        public bool ExecuteOnAuthority { get; }

        public SynchronizeCallAttribute(Authority target, bool executeOnAuthority = false)
        {
            Authority = target;
            ExecuteOnAuthority = executeOnAuthority;
        }
    }
}
