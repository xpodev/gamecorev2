using System;
using System.Collections.Generic;
using System.Xml;

namespace GameCore.Configuration
{
    public class XmlConfiguratorBase : IXmlConfigurator
    {
        protected Dictionary<string, Action<IXmlConfigurator, XmlNode>> actions = new Dictionary<string, Action<IXmlConfigurator, XmlNode>>();
        Action<IXmlConfigurator, XmlNode> defaultAction = null;

        public XmlConfiguratorBase()
        {
            RegisterAction("Include", IncludeXml);
        }

        protected void IncludeXml(IXmlConfigurator configurator, XmlNode node)
        {
            string path = node.Attributes["src"]?.Value;
            if (path is null)
            {
                return;
            }
            ExecuteFromPath(path);
        }

        public void RegisterAction(string name, Action<IXmlConfigurator, XmlNode> action)
        {
            if (actions.ContainsKey(name))
            {
                throw new ArgumentException($"Action \"{name}\" is already registered");
            }
            actions.Add(name, action);
        }

        public void SetDefaultAction(Action<IXmlConfigurator, XmlNode> action)
        {
            defaultAction = action;
        }

        public void ExecuteXml(XmlNode node)
        {
            try
            {
                actions[node.Name].Invoke(this, node);
            } catch (KeyNotFoundException)
            {
                if (defaultAction == null)
                {
                    throw new ArgumentException($"The input node \"{node.Name}\" type is not recognized");
                }
                defaultAction(this, node);
            }
        }

        public void ExecuteDocument(XmlElement rootElement)
        {
            foreach (XmlNode node in rootElement.ChildNodes)
            {
                ExecuteXml(node);
            }
        }

        public void ExecuteFromPath(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            ExecuteDocument(xml.DocumentElement);
        }
    }
}
