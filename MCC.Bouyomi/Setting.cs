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
        public static Setting Instance => instance ??= JsonSerializer.FileDeserialize<Setting>(FilePath);

        #endregion

        public string Format = "${UserName} ${Comment}";

        public string ApplicationPath;

        public bool Enable;
        public bool BlackListEnable;

        public List<BlackListItem> BlackListItems = new List<BlackListItem>();

        public void Save()
        {
            BlackListItems.Clear();
            BlackListItems.AddRange(BlackList.Instance.ToArray());
            JsonSerializer.FileSerialize<Setting>(FilePath, Setting.Instance);
        }
    }
}
