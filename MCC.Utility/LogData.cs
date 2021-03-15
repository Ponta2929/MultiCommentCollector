using System;

namespace MCC.Utility
{

    public enum LogLevel
    {
        Error,
        Warn,
        Info,
        Debug
    }

    public class LogData
    {
        public LogData(object sender, LogLevel level, DateTime date, string log) : this(sender.GetType().Name, level, date, log)
        {
        }

        public LogData(string sender, LogLevel level, DateTime date, string log)
        {
            SenderName = sender;
            Level = level;
            Date = date;
            Log = log;
        }

        /// <summary>
        /// 発信元
        /// </summary>
        public string SenderName { get; set; }

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
