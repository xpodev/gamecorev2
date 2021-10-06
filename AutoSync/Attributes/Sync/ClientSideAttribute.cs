using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ClientSideAttribute : AuthorityCodeAttribute
    {
        public ClientSideAttribute() : base(Authority.Client)
        {

        }
    }
}
