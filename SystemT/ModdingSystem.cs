using System;
using GameCore.Systems;
using GameCore.ModLoader;


namespace SystemT
{
    class ModdingSystem : SystemBase
    {
        public override string Name => "Modding System";

        public ModdingSystem(string modDir)
        {
            ModLoader = new ModLoader();
            DataDirectory = modDir;
        }

        public ModLoader ModLoader
        {
            get;
            private set;
        }

        public string DataDirectory
        {
            get;
            private set;
        }

        protected override void TurnOn()
        {
            ModLoader.Load(DataDirectory);
        }
    }
}
