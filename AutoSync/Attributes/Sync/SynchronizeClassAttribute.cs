using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SynchronizeClassAttribute : SynchronizeAttribute
    {
        public float MaxUpdateRate { get; set; }
    }
}
