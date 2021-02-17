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
        private LogManager logManager = LogManager.GetInstance();
        private object syncObjectLog = new();
        private object syncObjectComment = new();
        // ------------------------------------------------------------------------------------ //
        private string[] pluginList;

        public MultiCommentCollector()
        {
            var path = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins";

            if (Directory.Exists(path))
                pluginList = Directory.GetFiles($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins", "*.dll", SearchOption.AllDirectories);

        }

        /// <summary>
        /// 接続情報を適応します。
        /// </summary>
        public void Apply()
        {
            // プラグインロード
            foreach (var info in connectionManager.Items)
            {
                Apply(info);
            }
        }

        /// <summary>
        /// 接続情報を適応します。
        /// </summary>
        public void Apply(ConnectionData info)
        {
            if (info.Plugin is not null || pluginList is null)
                return;

            foreach (var plugin in pluginList)
            {
                var interfaces = PluginLoader.Load<IPluginSender>(plugin);

                if (interfaces is null)
                    continue;

                foreach (var @interface in interfaces)
                {
                    if (@interface is null || !info.PluginName.Equals(@interface.PluginName))
                        continue;

                    @interface.PluginLoad();

                    if (@interface.IsSupport(info.URL))
                    {
                        info.Plugin = @interface;

                        if (info.IsActive.Value)
                        {
                            Activate(info, true);
                        }
                    }
                    else
                    {
                        @interface.PluginClose();
                    }
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

            OnLogged(this, new($"プラグインを有効化しました。[{info.PluginName}]"));
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

            OnLogged(this, new($"プラグインを無効化しました。[{info.PluginName}]"));
        }

        public void AddURL(string url, bool isActive = false)
        {
            if (url.Length == 0)
                return;

            foreach (var plugin in pluginList)
            {
                var interfaces = PluginLoader.Load<IPluginSender>(plugin);

                if (interfaces is null)
                    continue;

                foreach (var @interface in interfaces)
                {
                    if (@interface is null)
                        continue;

                    @interface.PluginLoad();

                    if (@interface.IsSupport(url))
                    {
                        var info = new ConnectionData()
                        {
                            Plugin = @interface,
                            PluginName = @interface.PluginName,
                            IsActive = new(false),
                            URL = url
                        };

                        ConnectionManager.GetInstance().Items.Add(info);

                        OnLogged(this, new($"URLを追加しました。[{url}]"));

                        if (isActive)
                        {
                            Activate(info);
                        }
                    }
                    else
                    {
                        @interface.PluginClose();

                        OnLogged(this, new($"無効なURLが入力されました。[{url}]"));
                    }
                }
            }
        }
        private void OnLogged(object sender, LoggedEventArgs e)
        {
            lock (syncObjectLog)
            {
                if (sender is IPluginSender pluginSender)
                    logManager.Items.Add(new(pluginSender.PluginName, e.Date, e.Log));
                else
                    logManager.Items.Add(new(sender, e.Date, e.Log));
            }
            Debug.WriteLine($"[{sender.ToString()}] [{e.Date.ToShortTimeString()}] {e.Log}");

        }

        private void OnCommentReceived(object sender, CommentReceivedEventArgs e)
        {
            // コメントジェネレーターで送信
            generatorServer.SendData<CommentData>(e.CommentData);

            lock (syncObjectComment)
            {
                // コメント追加
                commentManager.Items.Add(e.CommentData);
            }
        }
    }
}
