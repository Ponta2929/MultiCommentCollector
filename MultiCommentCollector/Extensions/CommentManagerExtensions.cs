using MCC.Core.Manager;
using MCC.Utility;
using System.Linq;

namespace MultiCommentCollector.Extensions
{
    public static class CommentManagerExtensions
    {
        public static void Update(this CommentManager manager, UserData user)
        {
            var source = manager.Where(x => x.LiveName.Equals(user.LiveName) && x.UserID.Equals(user.UserID));

            foreach (var item in source)
            {
                item.BackColor = user.BackColor;
                item.UserName = user.UserName;
            }
        }
    }
}
