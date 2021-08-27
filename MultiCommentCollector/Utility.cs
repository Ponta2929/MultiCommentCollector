using MCC.Utility.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommentCollector
{
    public static class Utility
    {
        public static string ApplicationPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public static void SaveToXml<T>(string fileName, object @object)
        {
            try
            {
                XmlSerializer.FileSerialize<T>($"{ApplicationPath}\\{fileName}", @object);
            }
            catch
            {

            }
        }

        public static void SaveToJson<T>(string fileName, object @object)
        {
            try
            {
                JsonSerializer.FileSerialize<T>($"{ApplicationPath}\\{fileName}", @object);
            }
            catch
            {

            }
        }

        public static T LoadToXml<T>(string fileName) where T : new()
        {
            try
            {
                return XmlSerializer.FileDeserialize<T>($"{ApplicationPath}\\{fileName}");
            }
            catch
            {
                return new();
            }
        }

        public static T LoadToJson<T>(string fileName) where T : new()
        {
            try
            {
                return JsonSerializer.FileDeserialize<T>($"{ApplicationPath}\\{fileName}");
            }
            catch
            {
                return new();
            }
        }

        /// <summary>
        /// コピー用メニューを作成
        /// </summary>
        public static bool CreateMenuItemToCopy(MenuItem owner, string header)
        {
            if (header is null || header.Equals(""))
                return false;

            var content = new MenuItem();
            content.Header = header;
            content.Click += MenuItemCopy_Click;

            owner.Items.Add(content);

            return true;
        }

        /// <summary>
        /// コピー用メニューを作成(URL)
        /// </summary>
        public static bool CreateMenuItemToURL(MenuItem owner, string header)
        {
            var separator = false;
            var reg = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            var r = new Regex(reg, RegexOptions.IgnoreCase);

            foreach (var cut in header.Split(' '))
            {
                var collection = r.Matches(cut);

                foreach (Match m in collection)
                {
                    if (m.Success)
                    {
                        if (!separator)
                        {
                            owner.Items.Add(new Separator());
                            separator = true;
                        }

                        // コンテキストメニュー設定
                        CreateMenuItemToCopy(owner, m.Value);
                    }
                }
            }

            return separator;
        }

        private static void MenuItemCopy_Click(object sender, RoutedEventArgs _)
            => Clipboard.SetData(DataFormats.Text, (sender as MenuItem).Header);
    }
}
