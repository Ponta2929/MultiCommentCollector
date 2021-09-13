using MCC.Core.Manager;
using MCC.Utility.IO;
using Reactive.Bindings;
using System;
using System.IO;

namespace MultiCommentCollectorCLI
{
    public class Setting
    {
        #region Singleton

        private static Setting instance;
        public static Setting Instance => instance ??=
            XmlSerializer.FileDeserialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml");

        #endregion

        /// <summary>
        /// Servers
        /// </summary>
        public Server Servers { get; set; } = new();

        /// <summary>
        /// ConnectionList
        /// </summary>
        public ReactiveCollection<ConnectionData> ConnectionList { get; set; } = new();
    }
}
