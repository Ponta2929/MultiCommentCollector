using MCC.Utility;
using Reactive.Bindings;
using System;

namespace MultiCommentCollector.Models
{
    [Serializable]
    public class Theme
    {
        /// <summary>
        /// ダークモード
        /// </summary>
        public ReactiveProperty<bool> IsDarkMode { get; set; } = new(false);

        /// <summary>
        /// テーマカラー
        /// </summary>
        public ReactiveProperty<ColorData> ThemeColor { get; set; } = new(ColorData.FromArgb(255, 100, 149, 237));
    }
}