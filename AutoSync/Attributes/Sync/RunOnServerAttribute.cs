using System;

namespace GameCore.Net.Sync
{
    /// <summary>
    /// The method will be executed on the server
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RunOnServerAttribute : SynchronizeCallAttribute
    {
        public RunOnServerAttribute(bool executeOnAuthority = false) : base(Authority.Client, executeOnAuthority)
        {

        }
    }
}
