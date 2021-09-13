using MCC.Core.Manager;
using MCC.Core.Server;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using MultiCommentCollector.Views;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using System.Windows;

namespace MultiCommentCollector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly CompositeDisposable disposable = new();

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // Load XML Setting
            UserDataManager.SetInstance(SerializerHelper.XmlDeserialize<UserDataManager>("users.xml"));
            ConnectionManager.SetInstance(SerializerHelper.XmlDeserialize<ConnectionManager>("connections.xml"));
            Setting.SetInstance(SerializerHelper.XmlDeserialize<Setting>("setting.xml"));

            // MaxLimits
            CommentManager.Instance.MaxSize = Setting.Instance.ArrayLimits.MaxComments.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            LogManager.Instance.MaxSize = Setting.Instance.ArrayLimits.MaxLogs.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            // サーバー開始
            CommentGeneratorServer.Instance.Port = Setting.Instance.Servers.CommentGeneratorServerPort.Value;
            CommentReceiverServer.Instance.Port = Setting.Instance.Servers.CommentReceiverServerPort.Value;
            MCC.Core.Win.MultiCommentCollector.Instance.Apply();
            MCC.Core.Win.MultiCommentCollector.Instance.ServerStart();

            // Show MainWindow
            new MainWindow().Show();
        }

        private void ApplicationShutdown(object sender, ExitEventArgs e)
        {
            // サーバー停止
            MCC.Core.Win.MultiCommentCollector.Instance.ServerStop();

            // プラグイン終了処理
            foreach (var item in PluginManager.Instance)
            {
                item.PluginClose();
            }

            // 設定保存
            SerializerHelper.XmlSerialize("users.xml", UserDataManager.Instance);
            SerializerHelper.XmlSerialize("connections.xml", ConnectionManager.Instance);
            SerializerHelper.XmlSerialize("setting.xml", Setting.Instance);

            // Disposable
            disposable.Clear();
            disposable.Dispose();
        }
    }
}
