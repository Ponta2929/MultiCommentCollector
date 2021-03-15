using MCC.Core;
using MCC.Core.Manager;
using MCC.Utility.IO;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollectorCLI
{
    public class Setting
    {
        #region Singleton

        private static Setting instance;
        public static Setting GetInstance() => instance ??=
            XmlSerializer.FileDeserialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml");

        public static void SetInstance(Setting inst) => instance = inst;

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
