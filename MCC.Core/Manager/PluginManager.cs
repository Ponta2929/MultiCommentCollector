﻿using MCC.Plugin;
using MCC.Utility.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MCC.Core.Manager
{
    public class PluginManager : ListManagerBase<IPluginBase>
    {
        public ListManagerBase<IPluginBase> Parent = new();

        #region Singleton

        private static PluginManager instance;
        public static PluginManager GetInstance() => instance ??= new();
        public static void SetInstance(PluginManager inst) => instance = inst;

        #endregion


        public PluginManager()
        {
            IsLimit.Value = false;
        }

        public void Load(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException();

            var interfaceName = typeof(IPluginBase).Name;
            var plugins = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);

            foreach (var plugin in plugins)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(plugin);

                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetInterface(interfaceName) is not null)
                        {
                            Parent.Add((IPluginBase)assembly.CreateInstance(type.FullName));
                        }
                    }
                }
                catch
                {

                }
            }


            // IPluginReceiver
            var receiver = Parent.Where(x => x is IPluginReceiver).Select(x => CreateInstance(x)).ToArray();

            foreach (var item in receiver)
                item.PluginLoad();

            AddRange(receiver);
        }

        public IPluginBase CreateInstance(IPluginBase plugin)
        {
            try
            {
                var type = plugin.GetType();
                var assembly = type.Assembly;

                return assembly.CreateInstance(type.FullName) as IPluginBase;
            }
            catch
            {
                return null;
            }
        }
    }
}
