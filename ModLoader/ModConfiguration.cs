using System.IO;
using Newtonsoft.Json;


namespace GameCore.ModLoader
{
    public class ModConfiguration
    {
        [JsonProperty]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty]
        public string ConfigurationFile
        {
            get;
            set;
        }

        [JsonProperty]
        public string ParentDirectory
        {
            get;
            set;
        }

        [JsonProperty]
        public string[] Mods
        {
            get;
            set;
        }

        public string FullPath
        {
            get
            {
                return Path.Combine(ModLoader.CurrentLoader.BasePath, ParentDirectory);
            }
        }
    }
}
