using MCC.Core;
using MCC.Plugin;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector
{
    public class MenuItemViewModel
    {
        public string Key;

        public ReadOnlyReactiveCollection<IPluginBase> SubItem { get; }

        public MenuItemViewModel(string key)
        {
            this.Key = key;

            SubItem = PluginManager.GetInstance().ToReadOnlyReactiveCollection(x => x.PluginName == key ? x : null);
        }
    }
}
