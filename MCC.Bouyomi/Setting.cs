using MCC.Utility.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Bouyomi
{
    public class Setting
    {
        private static string FilePath = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins\\MCC.Bouyomi.json";

        #region Singleton

        private static Setting instance;
        public static Setting GetInstance() => instance ??=
            JsonSerializer.FileDeserialize<Setting>(FilePath);

        public static void SetInstance(Setting inst) => instance = inst;

        #endregion

        public string Format = "${UserName} ${Comment}";

        public string ApplicationPath;

        public bool Enable;

        public List<BlackListItem> BlackListItems = new List<BlackListItem>();

        public void Save()
        {
            BlackListItems.Clear();
            BlackListItems.AddRange(BlackList.GetInstance().ToArray());
            JsonSerializer.FileSerialize<Setting>(FilePath, Setting.GetInstance());
        }
    }
}
