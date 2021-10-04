using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ClientSideAttribute : AuthorityCodeAttribute
    {
        public new Authority Authority => Authority.Client;

        public ClientSideAttribute() : base(Authority.Client)
        {

        }
    }
}
