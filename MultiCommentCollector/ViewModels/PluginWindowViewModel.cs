using MCC.Core.Manager;
using MCC.Plugin;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModels
{
    internal class PluginWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<string> PluginName { get; }
        public ReactiveProperty<string> Author { get; }
        public ReactiveProperty<string> Description { get; }
        public ReactiveProperty<string> Version { get; }
        public ReactiveProperty<string> SiteName { get; }
        public ReactiveProperty<bool> Visibility { get; }

        public ReactiveCommand<IPluginBase> SelectedChangedCommand { get; }

        public CollectionViewSource ParentPluginsView { get; }

        public PluginWindowViewModel()
        {
            PluginName = new ReactiveProperty<string>("").AddTo(Disposable);
            Author = new ReactiveProperty<string>("").AddTo(Disposable);
            Description = new ReactiveProperty<string>("").AddTo(Disposable);
            Version = new ReactiveProperty<string>("").AddTo(Disposable);
            SiteName = new ReactiveProperty<string>("").AddTo(Disposable);
            Visibility = new ReactiveProperty<bool>(false).AddTo(Disposable);

            ParentPluginsView = new() { Source = PluginManager.Instance.Parent };

            SelectedChangedCommand = new ReactiveCommand<IPluginBase>().WithSubscribe(SetPluginInfo).AddTo(Disposable);
        }

        /// <summary>
        /// プラグイン情報設定
        /// </summary>
        private void SetPluginInfo(IPluginBase plugin)
        {
            PluginName.Value = plugin.PluginName;
            Author.Value = plugin.Author;
            Description.Value = plugin.Description;
            Version.Value = plugin.Version;

            if (plugin is IPluginSender sender)
            {
                Visibility.Value = true;
                SiteName.Value = sender.SiteName;
            }
            else
            {
                Visibility.Value = false;
            }
        }
    }
}
