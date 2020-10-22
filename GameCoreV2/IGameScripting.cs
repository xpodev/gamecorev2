using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.GameScripting
{
    public interface IGameScripting
    {
        void Execute(string source);
    }
}
