using System;

namespace GameCore.Net.Sync
{
    public class CustomFunctionCallAttribute : Attribute
    {
        public object[] Args { get; }

        public CustomFunctionCallAttribute(params object[] args)
        {
            Args = args;
        }
    }
}
