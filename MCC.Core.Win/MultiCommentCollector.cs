using MCC.Core.Manager;
using MCC.Plugin;
using MCC.Plugin.Win;
using System;
using System.Linq;
using System.Windows;

namespace MCC.Core.Win
{
    public class MultiCommentCollector : Core.MultiCommentCollector
    {
        #region Singleton

        private static MultiCommentCollector instance;
        public static new MultiCommentCollector GetInstance() => instance ?? (instance = new MultiCommentCollector());
        public static void SetInstance(MultiCommentCollector inst) => instance = inst;

        #endregion

        public void TestA(Window window)
        {
            var i = PluginManager.GetInstance().Where(x => x is IPluginReceiver).ToArray();

            foreach (var item in i)
            {
                if (item is ISetting setting)
                {
                    setting.ShowWindow(window);
                }
            }
        }
    }
}
