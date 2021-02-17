using MCC.Plugin;
using MCC.Utility.Reflection;
using System.IO;

namespace MCC.Core
{
    public class PluginManager : ListManagerBase<IPluginBase>
    {
        #region Singleton

        private static PluginManager instance;
        public static PluginManager GetInstance() => instance ??= new();
        public static void SetInstance(PluginManager inst) => instance = inst;

        #endregion

        public PluginManager()
        {
            IsLimit.Value = false;
        }

        public void プラグインとして有効なDLLを取得する関数(string folderPath)
        {
            Items.Clear();

            if (Directory.Exists(folderPath))
            {
                var pluginList = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);

                try
                {
                    foreach (var plugin in pluginList)
                    {
                        var impl = PluginLoader.Load<IPluginBase>(plugin);

                        if (impl is not null)
                        {
                            foreach (var item in impl)
                                Items.Add(item);
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}
