using MCC.Utility;

namespace MCC.Core
{
    public class CommentManager : ListManagerBase<CommentData>
    {
        #region Singleton

        private static CommentManager instance;
        public static CommentManager GetInstance() => instance ?? (instance = new());
        public static void SetInstance(CommentManager inst) => instance = inst;

        #endregion
    }
}
