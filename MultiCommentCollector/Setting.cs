﻿using MCC.Core;
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

namespace MultiCommentCollector
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
        /// MainWindow
        /// </summary>
        public WindowRect MainWindow { get; set; } = new(800, 600, 0, 0);

        /// <summary>
        /// LogWindow
        /// </summary>
        public WindowRect LogWindow { get; set; } = new(400, 300, 0, 0);

        /// <summary>
        /// Servers
        /// </summary>
        public Server Servers { get; set; } = new();

        /// <summary>
        /// MaxComments
        /// </summary>
        public ReactiveProperty<int> MaxComments { get; set; } = new(1000);

        /// <summary>
        /// MaxLogs
        /// </summary>
        public ReactiveProperty<int> MaxLogs { get; set; } = new(1000);

        /// <summary>
        /// IsPaneOpen
        /// </summary>
        public ReactiveProperty<bool> IsPaneOpen { get; set; } = new(true);

        /// <summary>
        /// PaneWidth
        /// </summary>
        public ReactiveProperty<int> PaneWidth { get; set; } = new(250);

        /// <summary>
        /// Theme
        /// </summary>
        public Theme Theme { get; set; } = new();

        /// <summary>
        /// ConnectionList
        /// </summary>
        public ReactiveCollection<ConnectionData> ConnectionList { get; set; } = new();
    }
}
