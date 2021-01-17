using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net
{
    public interface ICustomExtract<T> where T : struct, Enum
    {
        void Extract(Message<T> message);
    }
}
