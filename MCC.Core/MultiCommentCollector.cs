using MCC.Core.Server;
using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Core
{
    public class MultiCommentCollector
    {
        #region Singleton

        private static MultiCommentCollector instance;
        public static MultiCommentCollector GetInstance() => instance ?? (instance = new MultiCommentCollector());
        public static void SetInstance(MultiCommentCollector inst) => instance = inst;

        #endregion

        // ------------------------------------------------------------------------------------ //
        private CommentReceiverServer receiverServer = CommentReceiverServer.GetInstance();
        private CommentGeneratorServer generatorServer = CommentGeneratorServer.GetInstance();
        private ConnectionManager connectionManager = ConnectionManager.GetInstance();
        private CommentManager commentManager = CommentManager.GetInstance();
        private PluginManager pluginManager = PluginManager.GetInstance();
        private LogManager logManager = LogManager.GetInstance();
        // ------------------------------------------------------------------------------------ //

        public MultiCommentCollector()
        {
            var path ="";

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                path = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins";
            else
                path = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}/plugins";

            // プラグイン
            pluginManager.Load(path);
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

            OnLogged(this, new($"プラグインを有効化しました。[{info.Plugin.PluginName}]"));
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

            OnLogged(this, new($"プラグインを無効化しました。[{info.Plugin.PluginName}]"));
        }

        public void AddURL(string url, bool isActive = false)
        {
            if (url.Length == 0)
                return;

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

                    ConnectionManager.GetInstance().Add(info);

                    OnLogged(this, new($"URLを追加しました。[{url}]"));

                    if (isActive)
                    {
                        Activate(info);
                    }

                    pluginManager.Add(@interface);

                    break;
                }
                else
                {
                    @interface.PluginClose();

                    OnLogged(this, new($"無効なURLが入力されました。[{url}]"));
                }
            }
        }
        private void OnLogged(object sender, LoggedEventArgs e)
        {
            if (sender is IPluginBase pluginSender)
                logManager.SyncAdd(new(pluginSender.PluginName, e.Date, e.Log));
            else
                logManager.SyncAdd(new(sender, e.Date, e.Log));
        }

        private void OnCommentReceived(object sender, CommentReceivedEventArgs e)
        {
            // コメントジェネレーターで送信
            generatorServer.SendData<CommentData>(e.CommentData);

            // プラグインで送信
            foreach (var item in PluginManager.GetInstance())
            {
                if (item is IPluginReceiver receiver)
                    receiver.Receive(e.CommentData);
            }

            // コメント追加
            commentManager.SyncAdd(e.CommentData);
        }
    }
}
