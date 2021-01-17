using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.ModLoader
{
    public class ResourceDatabase
    {
        private readonly Dictionary<Type, Dictionary<string, object>> resources = new Dictionary<Type, Dictionary<string, object>>();

        public T[] GetResources<T>(bool removeNulls = true) where T : class
        {
            try
            {
                object[] objects = resources[typeof(T)].Values.ToArray();
                T[] result = new T[objects.Length];
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] is T res)
                    {
                        result[i] = res;
                    }
                }
                return removeNulls ? result.Where(obj => !(obj is null)).ToArray() : result;
            } catch (KeyNotFoundException)
            {
                return new T[0];
            }

        }

        public T GetResource<T>(string name) where T : class
        {
            try
            {
                return resources[typeof(T)][name] as T;
            } catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public void AddResource<T>(string name, T resource)
        {
            if (!resources.ContainsKey(typeof(T)))
            {
                resources.Add(typeof(T), new Dictionary<string, object>());
            }
            resources[typeof(T)].Add(name, resource);
        }
    }
}
