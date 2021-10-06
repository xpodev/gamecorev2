using System;
using System.Linq;


namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class NetworkConfigAttribute : Attribute
    {
        public Type ConfigurationType { get; set; }

        public NetworkConfigAttribute(Type configType)
        {
            if (configType is null)
            {
                throw new ArgumentNullException(nameof(configType));
            }
            if (!configType.IsClass)
            {
                throw new ArgumentException($"Argument must be a class", nameof(configType));
            }
            if (!(configType.GetCustomAttributes(typeof(NetworkManagerAttribute), true).FirstOrDefault() is NetworkManagerAttribute))
            {
                throw new ArgumentException($"Argument must have {typeof(NetworkManagerAttribute).Name} attribute", nameof(configType));
            }
            ConfigurationType = configType;
        }
    }
}
