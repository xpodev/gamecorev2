using System;

namespace GameCore.Net.Sync
{
    public class CustomFunctionCallAttribute : Attribute
    {
        public string[] Args { get; }

        public CustomFunctionCallAttribute(params string[] args)
        {
            Args = args;
        }
    }
}
