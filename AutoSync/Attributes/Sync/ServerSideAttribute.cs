using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ServerSideAttribute : AuthorityCodeAttribute
    {
        public new Authority Authority => Authority.Server;

        public ServerSideAttribute() : base(Authority.Server)
        {

        }
    }
}
