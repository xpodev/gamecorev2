using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.ModLoader
{
    class ResourceDatabase
    {
        private readonly Dictionary<Type, Dictionary<string, object>> resources = new Dictionary<Type, Dictionary<string, object>>();

        public T[] GetResources<T>() where T : class
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
            return result;
        }
    }
}
