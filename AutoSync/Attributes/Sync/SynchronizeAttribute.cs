using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public abstract class SynchronizeAttribute : Attribute
    {
        internal bool IsSynchronized { get; set; }
    }
}
