using MCC.Core.Manager;
using MCC.Plugin;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModel
{
    internal class PluginWindowViewModel : INotifyPropertyChanged, IDisposable
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        private readonly CompositeDisposable disposable = new();

        public void Dispose()
        {
            disposable.Clear();
            disposable.Dispose();
        }

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
            ParentPluginsView = new() { Source = PluginManager.Instance.Parent };

            PluginName = new ReactiveProperty<string>("").AddTo(disposable);
            Author = new ReactiveProperty<string>("").AddTo(disposable);
            Description = new ReactiveProperty<string>("").AddTo(disposable);
            Version = new ReactiveProperty<string>("").AddTo(disposable);
            SiteName = new ReactiveProperty<string>("").AddTo(disposable);
            Visibility = new ReactiveProperty<bool>(false).AddTo(disposable);

            SelectedChangedCommand = new ReactiveCommand<IPluginBase>().WithSubscribe(SetPluginInfo).AddTo(disposable);
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
