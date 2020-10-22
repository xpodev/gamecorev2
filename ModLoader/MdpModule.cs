using GameCore.Mdp;
using GameCore.Configuration;


namespace GameCore.ModLoader
{
    class MdpModule : ModuleBase
    {
        public override string Name => "Mdp";

        public MdpClient Mdp
        {
            get;
            private set;
        }

        public override void Setup(IXmlConfigurator configurator)
        {
            Mdp = new MdpClient();
            configurator.RegisterAction("Config", (_, node) => Mdp.Configure(node));
            configurator.RegisterAction("AddFiles", (_, node) => Mdp.AddFiles(node));
            configurator.RegisterAction("MdpVerify", (_, node) => Mdp.Run(ModLoader.CurrentLoader.Configuration.ParentDirectory));
        }
    }
}
