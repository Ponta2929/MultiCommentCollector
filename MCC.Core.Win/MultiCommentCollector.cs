using MCC.Core.Manager;
using MCC.Plugin;
using MCC.Plugin.Win;
using System;
using System.Linq;
using System.Windows;

namespace MCC.Core.Win
{
    public class MultiCommentCollector : Core.MultiCommentCollector
    {
        #region Singleton

        private static MultiCommentCollector instance;
        public static MultiCommentCollector Instance => instance ??= new();

        #endregion
    }
}
