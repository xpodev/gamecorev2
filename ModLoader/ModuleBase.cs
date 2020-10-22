using GameCore.Configuration;
using System.Xml;


namespace GameCore.ModLoader
{
    public abstract class ModuleBase
    {
        public abstract string Name
        {
            get;
        }

        public abstract void Setup(IXmlConfigurator configurator);
    }
}
