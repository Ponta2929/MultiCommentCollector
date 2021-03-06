﻿using MCC.Core;
using MCC.Plugin;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector
{
    public class PluginWindowViewModel : INotifyPropertyChanged, IDisposable
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


        public PluginWindowViewModel()
        {
            PluginName = new ReactiveProperty<string>("").AddTo(disposable);
            Author = new ReactiveProperty<string>("").AddTo(disposable);
            Description = new ReactiveProperty<string>("").AddTo(disposable);
            Version = new ReactiveProperty<string>("").AddTo(disposable);
            SiteName = new ReactiveProperty<string>("").AddTo(disposable);
            Visibility = new ReactiveProperty<bool>(false).AddTo(disposable);
            SelectedChangedCommand = new ReactiveCommand<IPluginBase>().WithSubscribe(x =>
            {
                PluginName.Value = x.PluginName;
                Author.Value = x.Author;
                Description.Value = x.Description;
                Version.Value = x.Version;

                if (x is IPluginSender sender)
                {
                    Visibility.Value = true;
                    SiteName.Value = sender.SiteName;
                }
                else
                {
                    Visibility.Value = false;
                }
            }).AddTo(disposable);
        }
    }
}
