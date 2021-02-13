using System;

namespace MCC.Utility
{
    public class LogData
    {
        public LogData(object sender, DateTime date, string log)
        {
            SenderName = sender.GetType().Name;
            Date = date;
            Log = log;
        }

        public LogData(string sender, DateTime date, string log)
        {
            SenderName = sender;
            Date = date;
            Log = log;
        }

        /// <summary>
        /// 発信元
        /// </summary>
        public string SenderName { get; set; }

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
