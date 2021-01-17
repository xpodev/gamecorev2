using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.ModLoader
{
    public class ModInfo
    {
        public ModInfo(int id, string dir, string name)
        {
            Id = id;
            Directory = dir;
            Name = name;
            Enabled = true;
        }

        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Directory
        {
            get;
            private set;
        }

        public bool Enabled
        {
            get;
            set;
        }
    }
}
