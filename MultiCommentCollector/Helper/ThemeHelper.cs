using ControlzEx.Theming;
using MaterialDesignThemes.Wpf;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using System.Windows.Media;

namespace MultiCommentCollector.Helper
{
    internal static class ThemeHelper
    {
        public static void ThemeChange(Color color, bool isDarkMode = false)
        {
            // Metro
            var colorName = ColorDataExtensions.FromColor(color).ToString();
            var colorBrush = new SolidColorBrush(color);
            if (ThemeManager.Current.GetTheme($"Dark.{colorName}") is null)
            {
                ThemeManager.Current.AddTheme(new ControlzEx.Theming.Theme($"Dark.{colorName}", $"Dark.{colorName}", "Dark", colorName, color, colorBrush, true, false));
                ThemeManager.Current.AddTheme(new ControlzEx.Theming.Theme($"Light.{colorName}", $"Light.{colorName}", "Light", colorName, color, colorBrush, true, false));
            }
            ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, isDarkMode ? $"Dark.{colorName}" : $"Light.{colorName}");

            // Material
            var ss = new MaterialDesignThemes.MahApps.MahAppsBundledTheme();
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(isDarkMode ? new MaterialDesignDarkTheme() : new MaterialDesignLightTheme());
            theme.SetPrimaryColor(color);
            paletteHelper.SetTheme(theme);
        }
    }
}
