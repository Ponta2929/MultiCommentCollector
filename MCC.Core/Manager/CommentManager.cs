using MCC.Utility;
using System.Linq;

namespace MCC.Core.Manager
{
    public class CommentManager : ListManagerBase<CommentDataEx>
    {
        #region Singleton

        private static CommentManager instance;
        public static CommentManager GetInstance() => instance ?? (instance = new());
        public static void SetInstance(CommentManager inst) => instance = inst;

        #endregion
    }
}
