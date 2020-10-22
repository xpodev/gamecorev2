using GameCore.Configuration;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace GameCore.ModLoader
{
    public class ModuleManager : ModuleBase
    {
        private readonly Dictionary<string, ModuleBase> modules = new Dictionary<string, ModuleBase>();

        private IXmlConfigurator Configurator;

        public override string Name => "ModuleManager";

        public void AddModuleFromXml(XmlNode node)
        {
            string moduleSource = node.Attributes["src"]?.Value;
            if (moduleSource is null)
            {
                return;
            }
            Assembly assembly = Assembly.LoadFrom(moduleSource);
            foreach (System.Type type in assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(ModuleBase))))
            {
                ModuleBase module = (ModuleBase)System.Activator.CreateInstance(type);
                AddModule(module);
            }
        }

        public void AddModule(ModuleBase module)
        {
            if (modules.ContainsKey(module.Name))
            {
                throw new System.ArgumentException($"Module(Name=\"{module.Name}\") already registered");
            }
            modules.Add(module.Name, module);
            module.Setup(Configurator);
        }

        public override void Setup(IXmlConfigurator configurator)
        {
            Configurator = configurator;
            configurator.RegisterAction("AddModule", (_, node) => AddModuleFromXml(node));
        }

        public void RequireModule(XmlNode node)
        {
            string moduleName = node.Attributes["name"]?.Value;
            if (moduleName is null)
            {
                return;
            }
            try
            {
                ModuleBase _ = modules[moduleName];
            } catch (KeyNotFoundException)
            {
                throw new System.Exception($"Required module \"{moduleName}\" not found");
            }
        }

        public T GetModule<T>() where T : ModuleBase
        {
            foreach (ModuleBase module in modules.Values)
            {
                if (module is T moduleT)
                {
                    return moduleT;
                }
            }
            return null;
        }
    }
}
