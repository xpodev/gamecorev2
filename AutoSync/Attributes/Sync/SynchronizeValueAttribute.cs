using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SynchronizeValueAttribute : SynchronizeCallAttribute
    {
        public SynchronizeValueAttribute(Authority authority, bool executeOnAuthority = true) : base(authority, executeOnAuthority)
        {

        }

        public string ConditionFunction { get; set; }
    }
}
