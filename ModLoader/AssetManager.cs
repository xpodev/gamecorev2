using GameCore.Configuration;
using System.Collections.Generic;
using System.Xml;


namespace GameCore.ModLoader
{
    // TODO: Implement AssetManager
    class AssetManager : ModuleBase
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

        }

        private void LoadModel(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadAudio(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadText(IXmlConfigurator _, XmlNode node)
        {

        }

        private void LoadData(IXmlConfigurator _, XmlNode node)
        {

        }

        private void RequireAsset(IXmlConfigurator _, XmlNode node)
        {

        }

        private void CreateAssetDatabase(IXmlConfigurator _, XmlNode __)
        {
            assets.Add(ModLoader.CurrentLoader.CurrentMod.Name, new ResourceDatabase());
        }

        public ResourceDatabase GetModAssets(string modName)
        {
            return assets[modName];
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
            configurator.RegisterAction("RequireAsset", RequireAsset);
            configurator.RegisterAction("CreateAssetDatabase", CreateAssetDatabase);
        }
    }
}
