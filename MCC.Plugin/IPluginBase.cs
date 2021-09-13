namespace MCC.Plugin
{
    public interface IPluginBase
    {
        /// <summary>
        /// 製作者名
        /// </summary>
        string Author { get; }

        /// <summary>
        /// プラグイン名です。
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// プラグインの詳細説明です。
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
