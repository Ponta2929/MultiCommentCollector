using MCC.Core.Manager;
using MCC.Core.Server;
using MCC.Plugin;
using MCC.Utility;
using System;
using System.IO;
using System.Linq;

namespace MCC.Core
{
    public class MultiCommentCollector
    {
        #region Singleton

        private static MultiCommentCollector instance;
        public static MultiCommentCollector Instance => instance ??= new();

        #endregion

        // ------------------------------------------------------------------------------------ //
        private CommentReceiverServer receiverServer = CommentReceiverServer.Instance;
        private CommentGeneratorServer generatorServer = CommentGeneratorServer.Instance;
        private ConnectionManager connectionManager = ConnectionManager.Instance;
        private CommentManager commentManager = CommentManager.Instance;
        private PluginManager pluginManager = PluginManager.Instance;
        private LogManager logManager = LogManager.Instance;
        private UserDataManager userDataManager = UserDataManager.Instance;
        // ------------------------------------------------------------------------------------ //

        public MultiCommentCollector()
        {
            var path = "";

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                path = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins";
            else
                path = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}/plugins";

            // プラグイン
            pluginManager.Load(path);

            // プラグインで送信
            foreach (var item in pluginManager)
            {
                if (item is IPluginReceiver receiver && item is ILogged log)
                {
                    log.OnLogged += OnLogged;
                }
            }
        }

        /// <summary>
        /// 接続情報を適応します。
        /// </summary>
        public void Apply()
        {
            // プラグインロード
            foreach (var info in connectionManager)
            {
                Apply(info);
            }
        }

        /// <summary>
        /// 接続情報を適応します。
        /// </summary>
        public void Apply(ConnectionData info)
        {
            if (info.Plugin is not null)
                return;

            var plugins = pluginManager.Parent.Where(x => x is IPluginSender).ToArray();

            foreach (var plugin in plugins)
            {
                var @interface = pluginManager.CreateInstance(plugin) as IPluginSender;

                @interface.PluginLoad();

                if (@interface.IsSupport(info.URL))
                {
                    info.Plugin = @interface;

                    if (info.IsActive.Value)
                    {
                        Activate(info, true);
                    }

                    pluginManager.Add(@interface);

                    break;
                }
                else
                {
                    @interface.PluginClose();
                }
            }
        }

        /// <summary>
        /// サーバー開始
        /// </summary>
        public void ServerStart()
        {
            // コメントサーバー
            receiverServer.OnLogged += OnLogged;
            receiverServer.OnCommentReceived += OnCommentReceived;
            receiverServer.Start();

            // コメントジェネレーター
            generatorServer.OnLogged += OnLogged;
            generatorServer.Start();
        }

        /// <summary>
        /// サーバー停止
        /// </summary>
        public void ServerStop()
        {
            // コメントサーバー
            receiverServer.OnLogged -= OnLogged;
            receiverServer.OnCommentReceived -= OnCommentReceived;
            receiverServer.Stop();

            // コメントジェネレーター
            generatorServer.OnLogged -= OnLogged;
            generatorServer.Stop();
        }

        /// <summary>
        /// プラグインを有効化
        /// </summary>
        public void Activate(ConnectionData info, bool forced = false)
        {
            if (info is null || info.IsActive.Value && !forced)
                return;

            if (info.Plugin is ILogged log)
                log.OnLogged += OnLogged;

            info.Plugin.OnCommentReceived += OnCommentReceived;
            info.IsActive.Value = info.Plugin.Activate();

            OnLogged(this, new(LogLevel.Info, $"プラグインを有効化しました。[{info.Plugin.PluginName}]"));
        }

        /// <summary>
        /// プラグインを無効化
        /// </summary>
        public void Inactivate(ConnectionData info)
        {
            if (info is null || !info.IsActive.Value)
                return;

            info.IsActive.Value = !info.Plugin.Inactivate();

            if (info.Plugin is ILogged log)
                log.OnLogged -= OnLogged;

            info.Plugin.OnCommentReceived -= OnCommentReceived;

            OnLogged(this, new(LogLevel.Info, $"プラグインを無効化しました。[{info.Plugin.PluginName}]"));
        }

        public void AddURL(string url, bool isActive = false)
        {
            if (url.Length == 0)
                return;

            var isSupport = false;
            var plugins = pluginManager.Parent.Where(x => x is IPluginSender).ToArray();

            foreach (var plugin in plugins)
            {
                var @interface = pluginManager.CreateInstance(plugin) as IPluginSender;

                @interface.PluginLoad();

                if (@interface.IsSupport(url))
                {
                    var info = new ConnectionData()
                    {
                        Plugin = @interface,
                        IsActive = new(false),
                        URL = url
                    };

                    connectionManager.Add(info);

                    OnLogged(this, new(LogLevel.Info, $"URLを追加しました。[{url}]"));

                    if (isActive)
                    {
                        Activate(info);
                    }

                    pluginManager.Add(@interface);

                    isSupport = true;

                    break;
                }
                else
                {
                    @interface.PluginClose();
                }
            }

            if (!isSupport)
                OnLogged(this, new(LogLevel.Info, $"無効なURLが入力されました。[{url}]"));
        }

        /// <summary>
        /// 接続状況を切り替え
        /// </summary>
        public void ToggleConnection(ConnectionData connection)
        {
            if (connection is null)
                return;

            if (connection.IsActive.Value)
                Inactivate(connection);
            else
                Activate(connection);
        }

        /// <summary>
        /// 接続を削除
        /// </summary>
        /// <param name="connection"></param>
        public void RemoveConnection(ConnectionData connection)
        {
            Inactivate(connection);

            // 削除
            pluginManager.Remove(connection.Plugin);
            connectionManager.Remove(connection);
        }

        private void OnLogged(object sender, LoggedEventArgs e)
        {
            if (sender is IPluginBase pluginSender)
                logManager.SyncAdd(new(pluginSender.PluginName, e.Level, e.Date, e.Log));
            else
                logManager.SyncAdd(new(sender, e.Level, e.Date, e.Log));
        }

        private void OnCommentReceived(object sender, CommentReceivedEventArgs e)
        {

            // ユーザー設定があるか
            var data = new CommentDataEx(e.CommentData);
            var userData = userDataManager.FirstOrDefault(x => x.LiveName.Equals(data.LiveName) && x.UserID.Equals(data.UserID));

            if (userData is not null)
                data.SetUserData(userData);

            // コメントジェネレーターで送信
            generatorServer.SendData(data);

            // プラグインで送信
            foreach (var item in pluginManager)
                if (item is IPluginReceiver receiver)
                    receiver.Receive(data);

            // コメント追加
            commentManager.SyncAdd(data);
        }
    }
}
