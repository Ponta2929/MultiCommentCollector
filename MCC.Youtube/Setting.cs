using MCC.Utility.IO;
using System;
using System.IO;

namespace MCC.Youtube
{
    public class Setting
    {
        private static string FilePath = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins\\MCC.Youtube.json";

        #region Singleton

        private static Setting instance;
        public static Setting Instance => instance ??= JsonSerializer.FileDeserialize<Setting>(FilePath);

        #endregion

        public string APIKey;

        public void Save() => JsonSerializer.FileSerialize<Setting>(FilePath, Setting.Instance);
    }
}
