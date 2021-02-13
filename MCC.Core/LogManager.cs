using MCC.Utility;

namespace MCC.Core
{
    public class LogManager : ListManagerBase<LogData>
    {
        #region Singleton

        private static LogManager instance;
        public static LogManager GetInstance() => instance ?? (instance = new());
        public static void SetInstance(LogManager inst) => instance = inst;

        #endregion
    }
}
