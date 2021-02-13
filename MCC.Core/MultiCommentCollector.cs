using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Reflection;
using System;
using System.Collections.Generic;
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
        // ------------------------------------------------------------------------------------ //
        private string[] pluginList = Directory.GetFiles($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins", "*.dll", SearchOption.AllDirectories);

        public MultiCommentCollector()
        {

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
            if (info.Plugin is not null)
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
        /// サーバー開始
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
        }

        public void AddURL(string url)
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

                        var t = connectionManager.Items;
                    }
                    else
                    {
                        @interface.PluginClose();
                    }
                }
            }
        }
        private void OnLogged(object sender, LoggedEventArgs e)
        {
            if (sender is IPluginSender pluginSender)
                logManager.Items.AddOnScheduler(new(pluginSender.PluginName, e.Date, e.Log));
            else
                logManager.Items.AddOnScheduler(new(sender, e.Date, e.Log));
        }

        private void OnCommentReceived(object sender, CommentReceivedEventArgs e)
        {
            // コメントジェネレーターで送信
            generatorServer.SendData<CommentData>(e.CommentData);

            // コメント追加
            commentManager.Items.AddOnScheduler(e.CommentData);
        }
    }
}
