using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Helper;
using Reactive.Bindings;
using System;

namespace MultiCommentCollector.Models
{
    [Serializable]
    public class Setting
    {
        #region Singleton

        private static Setting instance;
        public static Setting Instance => instance ??= new();
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
        /// ArrayLimits
        /// </summary>
        public ArrayLimit ArrayLimits { get; set; } = new();

        /// <summary>
        /// IsPaneOpen
        /// </summary>
        public Pane Pane { get; set; } = new();

        /// <summary>
        /// Theme
        /// </summary>
        public Theme Theme { get; set; } = new();
    }
}
