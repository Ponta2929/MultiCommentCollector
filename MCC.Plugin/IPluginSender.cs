using MCC.Utility;
using System.Xml.Serialization;

namespace MCC.Plugin
{
    public interface IPluginSender : IPluginBase
    {

        /// <summary>
        /// コメントをアプリケーション本体に送信する際に使用します。
        /// </summary>
        event CommentReceivedEventHandler OnCommentReceived;

        [XmlIgnore]
        /// <summary>
        /// サイト名です。
        /// </summary>
        string SiteName { get; }

        [XmlIgnore]
        /// <summary>
        /// サイト名です。
        /// </summary>
        string StreamKey { get; set; }

        /// <summary>
        /// プラグインがアクティブになると呼び出されます。
        /// </summary>
        bool Activate();

        /// <summary>
        /// プラグインが非アクティブになると呼び出されます。
        /// </summary>
        bool Inactivate();

        /// <summary>
        /// 対象のURLがこのプラグインに対応しているか。
        /// </summary>
        /// <param name="url">処理するURL</param>
        /// <returns>対応しているなら true.</returns>
        bool IsSupport(string url);
    }
}
