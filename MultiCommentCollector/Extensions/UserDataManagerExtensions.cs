using MCC.Core.Manager;
using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector.Extensions
{
    public static class UserDataManagerExtensions
    {
        /// <summary>
        /// 既存の項目があれば上書き、なければ追加
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        public static void Update(this UserDataManager manager, UserData user)
        {
            var userData = manager.FirstOrDefault(x => x.LiveName.Equals(user.LiveName) && x.UserID.Equals(user.UserID));

            if (userData is not null)
            {
                userData.HideUser = user.HideUser;
                userData.UserName = user.UserName;
                userData.BackColor = user.BackColor;
            }
            else
            {
                manager.Add(user);
            }
        }

        public static bool Remove(this UserDataManager manager, CommentManager commentManager, UserData user)
        {
            var source = commentManager.Where(x => x.LiveName.Equals(user.LiveName) && x.UserID.Equals(user.UserID)).ToArray();

            foreach (var item in source)
            {
                item.BackColor = user.BeforeBackColor;
                item.UserName = user.BeforeUserName;
            }

            return manager.Remove(user);
        }

        public static UserData Find(this UserDataManager manager, CommentDataEx commentDataEx)
            => manager.FirstOrDefault(x => x.LiveName.Equals(commentDataEx.LiveName) && x.UserID.Equals(commentDataEx.UserID));
    }
}
