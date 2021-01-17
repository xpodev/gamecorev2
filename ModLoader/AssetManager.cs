using GameCore.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.IO;


namespace GameCore.ModLoader
{
    // TODO: Implement AssetManager
    public class AssetManager : ModuleBase
    {
        private readonly Dictionary<string, ResourceDatabase> assets = new Dictionary<string, ResourceDatabase>();

        public override string Name => "AssetManager";

        private void LoadAsset(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadTexture(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadAssembly(IXmlConfigurator _, XmlNode node)
        {
            // TODO: reimplement this, create better exceptions
            string path = node.Attributes["src"]?.Value;
            string name = node.Attributes["name"]?.Value;
            if (path is null)
            {
                Debug.Fail($"tried to load an assembly, but no path was given");
                return;
            }
            if (name is null)
            {
                Debug.Fail($"tried to load an assembly, but no name was given");
                return;
            }
            Assembly assembly = Assembly.LoadFrom(Path.Combine(ModLoader.CurrentLoader.CurrentMod.Directory, path));
            GetModAssets(ModLoader.CurrentLoader.CurrentMod.Name).AddResource(name, assembly);
        }

        private void LoadModel(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadAudio(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadText(IXmlConfigurator _, XmlNode node)
        {
            // TODO: reimplement this, create better exceptions
            string path = node.Attributes["src"]?.Value;
            string name = node.Attributes["name"]?.Value;
            if (path is null)
            {
                Debug.Fail($"tried to load a text file, but no path was given");
                return;
            }
            if (name is null)
            {
                Debug.Fail($"tried to load a text file, but no name was given");
                return;
            }
            string data = File.ReadAllText(Path.Combine(ModLoader.CurrentLoader.CurrentMod.Directory, path));
            GetModAssets(ModLoader.CurrentLoader.CurrentMod.Name).AddResource(name, data);
        }

        private void LoadData(IXmlConfigurator _, XmlNode node)
        {
            // TODO: reimplement this, create better exceptions
            string path = node.Attributes["src"]?.Value;
            string name = node.Attributes["name"]?.Value;
            if (path is null)
            {
                Debug.Fail($"tried to load a data file, but no path was given");
                return;
            }
            if (name is null)
            {
                Debug.Fail($"tried to load a data file, but no name was given");
                return;
            }
            byte[] data = File.ReadAllBytes(Path.Combine(ModLoader.CurrentLoader.CurrentMod.Directory, path));
            GetModAssets(ModLoader.CurrentLoader.CurrentMod.Name).AddResource(name, data);
        }

        private void LoadBundle(IXmlConfigurator _, XmlNode node)
        {
            // TODO: reimplement this, create better exceptions
            string path = node.Attributes["src"]?.Value;
            string name = node.Attributes["name"]?.Value;
            if (path is null)
            {
                Debug.Fail($"tried to load a data file, but no path was given");
                return;
            }
            if (name is null)
            {
                Debug.Fail($"tried to load a data file, but no name was given");
                return;
            }
            UnityEngine.AssetBundle data = UnityEngine.AssetBundle.LoadFromFile(Path.Combine(ModLoader.CurrentLoader.CurrentMod.Directory, path));
            GetModAssets(ModLoader.CurrentLoader.CurrentMod.Name).AddResource(name, data);
        }

        private void RequireAsset(IXmlConfigurator _, XmlNode node)
        {
            string path = node.Attributes["path"]?.Value;
            if (path is null)
            {
                throw new System.ArgumentException($"Invalid asset path [Empty]");
            }
            string[] parts = path.Split(':');
            if (parts.Length != 2)
            {
                throw new System.ArgumentException($"Invalid asset path \"{path}\". Must be in the format \"ModName:AssetName\"");
            }
        }

        private void CreateAssetDatabase(IXmlConfigurator _, XmlNode __)
        {
            assets.Add(ModLoader.CurrentLoader.CurrentMod.Name, new ResourceDatabase());
        }

        public ResourceDatabase GetModAssets(string modName)
        {
            return assets[modName];
        }

        public T GetModAsset<T>(string modName, string assetName) where T : class
        {
            ResourceDatabase resourceDatabase = GetModAssets(modName);
            return resourceDatabase.GetResource<T>(assetName);
        }

        public ResourceDatabase GetModAssets()
        {
            return assets[ModLoader.CurrentLoader.CurrentMod.Name];
        }

        public T GetModAsset<T>(string assetName) where T : class
        {
            ResourceDatabase resourceDatabase = GetModAssets(ModLoader.CurrentLoader.CurrentMod.Name);
            return resourceDatabase.GetResource<T>(assetName);
        }

        public override void Setup(IXmlConfigurator configurator)
        {
            configurator.RegisterAction("Asset", LoadAsset);
            configurator.RegisterAction("Texture", LoadTexture);
            configurator.RegisterAction("Assembly", LoadAssembly);
            configurator.RegisterAction("Model", LoadModel);
            configurator.RegisterAction("Audio", LoadAudio);
            configurator.RegisterAction("Text", LoadText);
            configurator.RegisterAction("BinaryData", LoadData);
            configurator.RegisterAction("Bundle", LoadBundle);
            configurator.RegisterAction("RequireAsset", RequireAsset);
            configurator.RegisterAction("CreateAssetDatabase", CreateAssetDatabase);
        }
    }
}
