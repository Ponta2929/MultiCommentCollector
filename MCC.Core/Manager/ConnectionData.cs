using MCC.Plugin;
using Reactive.Bindings;
using System;
using System.Xml.Serialization;

namespace MCC.Core.Manager
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
        /// 処理するURL
        /// </summary>
        [XmlIgnore]
        public string StreamKey { get; set; }

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
