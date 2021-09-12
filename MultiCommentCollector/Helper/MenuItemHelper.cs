using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommentCollector.Helper
{
    public static class MenuItemHelper
    {
        /// <summary>
        /// コピー用メニューを作成
        /// </summary>
        public static bool CreateMenuItemToCopy(MenuItem owner, string header)
        {
            if (header is null || header.Equals(""))
                return false;

            var content = new MenuItem();
            content.Header = header;
            content.Click += (sender, _) =>
                Clipboard.SetData(DataFormats.Text, (sender as MenuItem).Header);

            owner.Items.Add(content);

            return true;
        }

        /// <summary>
        /// コピー用メニューを作成(URL)
        /// </summary>
        public static bool CreateMenuItemToCopyURL(MenuItem owner, string header, bool separator = true)
        {
            if (header is null || header.Equals(""))
                return false;

            var created = false;
            var reg = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            var r = new Regex(reg, RegexOptions.IgnoreCase);

            foreach (var cut in header.Split(' '))
            {
                var collection = r.Matches(cut);

                foreach (Match m in collection)
                {
                    if (m.Success)
                    {
                        if (separator && !created)
                        {
                            owner.Items.Add(new Separator());
                            created = true;
                        }

                        // コンテキストメニュー設定
                        CreateMenuItemToCopy(owner, m.Value);
                    }
                }
            }

            return true;
        }
    }
}
