namespace GameCore.ModLoader
{
    public abstract class Mod
    {
        public ModInfo Info
        {
            get;
            private set;
        }

        public AssetManager AssetManager
        {
            get
            {
                return ModLoader.CurrentLoader.ModuleManager.GetModule<AssetManager>();
            }
        }

        public void SetInfo(ModInfo info)
        {
            Info = info;
        }

        public abstract void Initialize(ModLoader loader);
    }
}
