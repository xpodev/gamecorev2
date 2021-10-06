using System;

namespace GameCore.Net.Sync
{
    /// <summary>
    /// This attribute tells this property is controlled by the owning client.
    /// <para>This is not recommended</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SetFromClientAttribute : SynchronizeValueAttribute
    {
        public SetFromClientAttribute(bool executeOnAuthority = true) : base(Authority.Client, executeOnAuthority)
        {

        }
    }
}
