using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Newtonsoft.Json;


namespace GameCore.ModLoader
{
    public sealed class ModLoader : IModLoader
    {
        public static ModLoader CurrentLoader
        {
            get;
            private set;
        }

        public enum LoaderState
        {
            Standby,
            LoadingConfig,
            ExecutingConfig,
            CreatingModList,
            PreloadMods,
            VerifyMods,
            LoadMods,
            InitializeMods,
            FinishLoad
        }

        private const string ModsConfigurationFileName = "ModsConfigurations.json";

        private const string ModPreLoadConfigurationFileName = "PreLoad.xml";
        private const string ModVerifyConfigurationFileName = "Verify.xml";
        private const string ModLoadConfigurationFileName = "Load.xml";

        private static readonly ModConfiguration DefaultConfig = new ModConfiguration()
        {
            Name = "GameCore.DefaultConfiguration",
            Mods = new string[0],
            ConfigurationFile = ""
        };

        public ModConfiguration Configuration
        {
            get;
            private set;
        }

        public ModLoaderConfigurator Configurator
        {
            get;
            private set;
        }

        public LoaderState State
        {
            get;
            private set;
        }

        public ModInfo[] Mods
        {
            get;
            private set;
        }

        public ModInfo CurrentMod
        {
            get;
            private set;
        }

        public ModuleManager ModuleManager
        {
            get;
            private set;
        }

        public string BasePath
        {
            get;
            private set;
        }

        public ModLoader()
        {
            State = LoaderState.Standby;
            CurrentLoader = this;

            BasePath = Directory.GetCurrentDirectory();

            Configurator = new ModLoaderConfigurator();
            ModuleManager = new ModuleManager();
            ModuleManager.Setup(Configurator);

            ModuleManager.AddModule(new AssetManager());
            ModuleManager.AddModule(new MdpModule());

            Configurator.RegisterAction("EnableMod", (_, node) => EnableMod(node));
            Configurator.RegisterAction("DisableMod", (_, node) => DisableMod(node));
            Configurator.RegisterAction("RequireMod", (_, node) => RequireMod(node));
        }

        public void Load(string path)
        {
            if (State != LoaderState.Standby)
            {
                string stateName = Enum.GetName(typeof(LoaderState), State);
                throw new InvalidOperationException($"Tries to load ModLoader while not in \"Standby\" ({stateName})");
            }

            NextState();
            Configuration = LoadConfig(Path.Combine(path, ModsConfigurationFileName));
            NextState();
            if (!string.IsNullOrWhiteSpace(Configuration.ConfigurationFile))
            {
                ExecuteConfigurationFile(Path.Combine(path, Configuration.ParentDirectory, Configuration.ConfigurationFile));
            }
            NextState();
            Mods = GetMods(path);
            NextState();
            PreLoadMods();
            NextState();
            VerifyMods();
            NextState();
            LoadMods();
            NextState();
            InitializeMods();
            NextState();
            if (State != LoaderState.FinishLoad)
            {
                string stateName = Enum.GetName(typeof(LoaderState), State);
                throw new Exception($"Expected that the loader will finish with the LoaderState.FinishLoad state, but it was {stateName}");
            }
        }

        private void PreLoadMods()
        {
            foreach (ModInfo mod in Mods)
            {
                if (!mod.Enabled)
                {
                    continue;
                }
                CurrentMod = mod;
                string path = Path.Combine(mod.Directory, ModPreLoadConfigurationFileName);
                if (File.Exists(path))
                {
                    Configurator.ExecuteFromPath(path);
                }
            }
        }

        private void VerifyMods()
        {
            foreach (ModInfo mod in Mods)
            {
                if (!mod.Enabled)
                {
                    continue;
                }
                CurrentMod = mod;
                string path = Path.Combine(mod.Directory, ModVerifyConfigurationFileName);
                if (File.Exists(path))
                {
                    Configurator.ExecuteFromPath(path);
                }
            }
        }

        private void LoadMods()
        {
            foreach (ModInfo mod in Mods)
            {
                if (!mod.Enabled)
                {
                    continue;
                }
                CurrentMod = mod;
                string path = Path.Combine(mod.Directory, ModLoadConfigurationFileName);
                if (File.Exists(path))
                {
                    Configurator.ExecuteFromPath(path);
                }
            }
        }

        private void InitializeMods()
        {
            AssetManager assetManager = ModuleManager.GetModule<AssetManager>();
            if (assetManager is null)
            {
                return;
            }
            foreach (ModInfo modInfo in Mods)
            {
                if (!modInfo.Enabled)
                {
                    continue;
                }
                CurrentMod = modInfo;
                foreach (Assembly assembly in assetManager.GetModAssets(modInfo.Name).GetResources<Assembly>())
                {
                    Type modType = assembly.GetTypes().Where(t => typeof(Mod).IsAssignableFrom(t)).FirstOrDefault();
                    if (modType is null)
                    {
                        continue;
                    }
                    Mod modInstance = (Mod)Activator.CreateInstance(modType);
                    modInstance.SetInfo(modInfo);
                    modInstance.Initialize(this);
                }
            }
        }

        private void NextState()
        {
            State++;
            CurrentMod = null;
        }

        private ModInfo[] GetMods(string path)
        {
            ModInfo[] mods = new ModInfo[Configuration.Mods.Length];
            for (int i = 0; i < mods.Length; i++)
            {
                string modFolder = Path.Combine(path, Configuration.ParentDirectory, Configuration.Mods[i]);
                string name = Path.GetFileName(Path.GetDirectoryName(modFolder));
                ModInfo mod = new ModInfo(i, modFolder, name);
                mods[i] = mod;
            }
            return mods;
        }

        private void ExecuteConfigurationFile(string path)
        {
            Configurator.ExecuteFromPath(path);
        }

        private ModConfiguration LoadConfig(string path)
        {
            string configContent = File.ReadAllText(path);
            ModLoaderConfiguration modLoaderConfiguration = JsonConvert.DeserializeObject<ModLoaderConfiguration>(configContent);
            ModConfiguration selectedConfig = null;
            foreach (ModConfiguration configuration in modLoaderConfiguration.Configurations)
            {
                if (configuration.Name == modLoaderConfiguration.CurrentConfiguration)
                {
                    selectedConfig = configuration;
                    break;
                }
            }
            if (selectedConfig == null)
            {
                selectedConfig = DefaultConfig;
            }
            return selectedConfig;
        }

        // TODO: print warnings
        private void RequireMod(XmlNode node)
        {
            string modName = node.Attributes["name"]?.Value;
            if (modName is null)
            {
                return;
            }
            if (State >= LoaderState.LoadMods)
            {
                return;
            }
            ModInfo modInfo = Mods.Where(mod => mod.Name == modName).FirstOrDefault();
            if (modInfo is null)
            {
                // TODO: create a custom exception
                throw new Exception("mod not found");
            }
            if (!modInfo.Enabled)
            {
                // TODO: create a custom exception
                throw new Exception("mod is disabled");
            }
        }

        // TODO: print warnings
        private void DisableMod(XmlNode node)
        {
            string modName = node.Attributes["name"]?.Value;
            if (modName is null)
            {
                return;
            }
            if (State >= LoaderState.LoadMods)
            {
                return;
            }
            ModInfo modInfo = Mods.Where(mod => mod.Name == modName).FirstOrDefault();
            if (modInfo is null)
            {
                return;
            }
            modInfo.Enabled = false;
        }

        // TODO: print warnings
        private void EnableMod(XmlNode node)
        {
            string modName = node.Attributes["name"]?.Value;
            if (modName is null)
            {
                return;
            }
            if (State >= LoaderState.LoadMods)
            {
                return;
            }
            ModInfo modInfo = Mods.Where(mod => mod.Name == modName).FirstOrDefault();
            if (modInfo is null)
            {
                return;
            }
            modInfo.Enabled = true;
        }
    }
}
