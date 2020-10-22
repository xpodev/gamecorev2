using Newtonsoft.Json;


namespace GameCore.ModLoader
{
    public struct ModLoaderConfiguration
    {
        [JsonProperty]
        public string CurrentConfiguration
        {
            get;
            private set;
        }

        [JsonProperty]
        public ModConfiguration[] Configurations
        {
            get;
            private set;
        }
    }
}
