using MCC.Utility.IO;
using System;
using System.IO;

namespace MCC.Twitch
{
    public class Setting
    {
        private static string FilePath = $"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\plugins\\MCC.Twitch.json";

        #region Singleton

        private static Setting instance;
        public static Setting Instance => instance ??= JsonSerializer.FileDeserialize<Setting>(FilePath);

        #endregion

        public string Password;

        public void Save()
        {
            JsonSerializer.FileSerialize<Setting>(FilePath, Setting.Instance);
        }
    }
}
