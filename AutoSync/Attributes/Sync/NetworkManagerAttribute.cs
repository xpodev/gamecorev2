using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    class NetworkManagerAttribute: Attribute
    {
        public Type NetworkManagerType { get; }

        public NetworkManagerAttribute(Type networkManagerType)
        {
            NetworkManagerType = networkManagerType;
        }
    }
}
