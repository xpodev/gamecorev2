using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Multiplayer
{
    public class DefaultClock : IClock
    {
        public long GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks;
        }
    }
}
