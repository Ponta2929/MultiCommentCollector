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
