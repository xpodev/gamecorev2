using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.NetAlpha
{
    public abstract class CustomMessageHandler
    {
        public CustomMessageHandler() { }

        public abstract void InsertInto<T>(Message<T> message) where T : struct, Enum;

        public abstract void ExtractFrom<T>(Message<T> message) where T : struct, Enum;
    }
}
