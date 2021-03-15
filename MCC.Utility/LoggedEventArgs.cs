using System;

namespace MCC.Utility
{
    public delegate void LoggedEventHandler(object sender, LoggedEventArgs e);

    /// <summary>
    /// ログイベント
    /// </summary>
    public class LoggedEventArgs : EventArgs
    {
        public LoggedEventArgs(LogLevel level, string log) : this(level, DateTime.Now, log) { }

        public LoggedEventArgs(LogLevel level, DateTime date, string log) =>
            (Level, Date, Log) = (level, date, log);

        /// <summary>
        /// レベル
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 発信時刻
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// ログ内容
        /// </summary>
        public string Log { get; set; }
    }
}
