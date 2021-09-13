using System.Windows;

namespace MCC.Plugin.Win
{
    public interface ISetting : IPluginBase
    {
        string MenuItemName { get; }

        void ShowWindow(Window window);
    }
}
