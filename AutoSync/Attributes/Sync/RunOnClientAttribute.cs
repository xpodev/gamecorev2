using System;

namespace GameCore.Net.Sync
{
    /// <summary>
    /// The method will be executed on the client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RunOnClientAttribute : SynchronizeCallAttribute
    {
        public new Authority Authority => Authority.Server;

        public RunOnClientAttribute(bool executeOnAuthority = false) : base(Authority.Server, executeOnAuthority)
        {

        }
    }
}
