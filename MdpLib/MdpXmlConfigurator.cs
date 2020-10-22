using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Configuration;


namespace GameCore.Mdp
{
    class MdpXmlConfigurator : XmlConfiguratorBase
    {
        public MdpClient Mdp
        {
            get;
            private set;
        }

        public MdpXmlConfigurator()
        {
            Mdp = new MdpClient();
            RegisterAction("Config", (_, node) => Mdp.Configure(node));
            RegisterAction("AddFiles", (_, node) => Mdp.LoadXml(node));
        }
    }
}
