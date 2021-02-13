using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MCC.Utility.Reflection
{
    public static class PluginLoader
    {
        public static T[] Load<T>(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            var interfaceName = typeof(T).Name;
            var interfaces = new List<T>();

            try
            {
                var assembly = Assembly.LoadFrom(fileName);

                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetInterface(interfaceName) != null)
                    {
                        interfaces.Add((T)assembly.CreateInstance(type.FullName));
                    }
                }
            }
            catch
            {
                return null;
            }

            return interfaces.ToArray();
        }
    }
}
