using MCC.Core;
using MCC.Utility.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiCommentCollector
{
    public static class WindowManager
    {
        public static void ApplicationStart()
        {
            Setting setting = Setting.GetInstance();

            Application.Current.MainWindow.Closing += (_, _) => ApplicationShutdown();

            foreach (var item in Setting.GetInstance().ConnectionList)
                ConnectionManager.GetInstance().Items.Add(item);

            MCC.Core.MultiCommentCollector.GetInstance().Apply();
            MCC.Core.MultiCommentCollector.GetInstance().ServerStart();
        }

        public static void ShowLogWindow() => LogWindow.GetInstance().Show();

        public static void ShowOptionWindow()
        {
            var option = new OptionWindow();
            option.Owner = Application.Current.MainWindow;
            option.ShowDialog();
        }

        public static void ApplicationShutdown()
        {
            MCC.Core.MultiCommentCollector.GetInstance().ServerStop();

            Setting.GetInstance().ConnectionList = ConnectionManager.GetInstance().Items;

            try
            {
                XmlSerializer.FileSerialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml", Setting.GetInstance());
            }
            catch
            {

            }

            LogWindow.GetInstance().IsOwnerClose = true;
        }
    }
}
