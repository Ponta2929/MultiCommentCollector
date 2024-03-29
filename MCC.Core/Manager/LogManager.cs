﻿using MCC.Utility;

namespace MCC.Core.Manager
{
    public class LogManager : ListManagerBase<LogData>
    {
        #region Singleton

        private static LogManager instance;
        public static LogManager Instance => instance ??= new();
        public static void SetInstance(LogManager inst) => instance = inst;

        #endregion
    }
}
