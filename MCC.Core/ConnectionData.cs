using MCC.Plugin;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MCC.Core
{
    /// <summary>
    /// 接続先情報
    /// </summary>
    [Serializable]
    public class ConnectionData
    {
        /// <summary>
        /// プラグインのインスタンス
        /// </summary>
        [XmlIgnore]
        public IPluginSender Plugin { get; set; }

        /// <summary>
        /// プラグイン名
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// 処理するURL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// プラグインのアクティブ状態
        /// </summary>
        public ReactiveProperty<bool> IsActive { get; init; } = new(false);
    }
}
