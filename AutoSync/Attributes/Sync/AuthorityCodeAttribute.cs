using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class AuthorityCodeAttribute : SynchronizeAttribute
    {
        public Authority Authority { get; }

        public AuthorityCodeAttribute(Authority authority)
        {
            Authority = authority;
        }
    }
}
