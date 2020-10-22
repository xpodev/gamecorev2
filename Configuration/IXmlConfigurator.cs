using System;
using System.Xml;

namespace GameCore.Configuration
{
    public interface IXmlConfigurator
    {
        void RegisterAction(string name, Action<IXmlConfigurator, XmlNode> action);

        void SetDefaultAction(Action<IXmlConfigurator, XmlNode> action);

        void ExecuteXml(XmlNode node);

        void ExecuteDocument(XmlElement rootElement);

        void ExecuteFromPath(string path);
    }
}
