using System;

namespace GameCore.Net.Sync.Internal
{
    internal class CustomCallWrapper
    {
        public object[] Arguments { get; }

        public CustomCallWrapper(CustomFunctionCallAttribute customCall)
        {
            if (customCall == null)
                throw new ArgumentNullException(nameof(customCall));

            Arguments = customCall.Args;
        }
    }
}
