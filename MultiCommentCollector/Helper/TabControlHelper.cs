using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommentCollector.Helper
{
    public static class TabControlHelper
    {
        /// <summary>
        /// 子ウィンドウのヘッダーサイズを設定
        /// </summary>
        public static void SetHeaderFontSize(DependencyObject element, double size)
        {
            if (element is null)
            {
                return;
            }

            foreach (var child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is DependencyObject control)
                {
                    if (control is TabControl tabControl)
                    {
                        HeaderedControlHelper.SetHeaderFontSize(tabControl, size);
                    }

                    SetHeaderFontSize(child as DependencyObject, size);
                }
            }
        }
    }
}
