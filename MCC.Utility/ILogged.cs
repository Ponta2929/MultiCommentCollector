namespace MCC.Utility
{
    /// <summary>
    /// ログ
    /// </summary>
    public interface ILogged
    {
        /// <summary>
        /// ログデータを送信してください。
        /// </summary>
        event LoggedEventHandler OnLogged;
    }
}
