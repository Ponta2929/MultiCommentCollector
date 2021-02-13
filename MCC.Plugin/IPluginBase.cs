using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Plugin
{
    public interface IPluginBase
    {
        /// <summary>
        /// プラグイン名です。
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// プラグイン名の詳細説明です。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// プラグインバージョンです。
        /// </summary>
        string Version { get; }

        /// <summary>
        /// プラグイン読み込み時に呼ばれます。
        /// </summary>
        void PluginLoad();

        /// <summary>
        /// プラグイン終了時に呼ばれます。
        /// </summary>
        void PluginClose();
    }
}
