using ControlzEx.Theming;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Xml.Serialization;

namespace MultiCommentCollector
{
    [Serializable]
    public class Theme
    {
        [XmlIgnore]
        public string[] Colors = ThemeManager.Current.Themes.Where(x => x.BaseColorScheme.Equals("Dark")).Select(x => x.ColorScheme).ToArray();

        /// <summary>
        /// ダークモード
        /// </summary>
        public ReactiveProperty<bool> IsDarkMode { get; set; } = new(false);

        /// <summary>
        /// テーマカラー
        /// </summary>
        public ReactiveProperty<string> ThemeColor { get; set; } = new("Blue");
    }
}