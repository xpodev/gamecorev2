﻿using System;

namespace GameCore.Net.Sync
{
    /// <summary>
    /// This attribute tells this property is controlled by the server
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SetFromServerAttribute : SynchronizeValueAttribute
    {
        public SetFromServerAttribute(bool executeOnAuthority = true) : base(Authority.Server, executeOnAuthority)
        {

        }
    }
}
