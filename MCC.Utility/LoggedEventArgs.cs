using System;

namespace MCC.Utility
{
    public delegate void LoggedEventHandler(object sender, LoggedEventArgs e);

    /// <summary>
    /// ログイベント
    /// </summary>
    public class LoggedEventArgs : EventArgs
    {
        public LoggedEventArgs(string log)
        {
            Date = DateTime.Now;
            Log = log;
        }

        /// <summary>
        /// ログ時間
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// ログ内容
        /// </summary>
        public string Log { get; private set; }
    }
}
