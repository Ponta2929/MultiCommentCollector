using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MCC.Plugin.Win
{
    public interface ISetting : IPluginBase
    {
        string MenuItemName { get;  }

        void ShowWindow(Window window);
    }
}
