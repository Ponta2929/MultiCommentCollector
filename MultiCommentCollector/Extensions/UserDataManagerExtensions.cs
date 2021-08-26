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
            var userData = manager.Where(x => x.LiveName.Equals(user.LiveName) && x.UserID.Equals(user.UserID)).ToArray();

            if (userData.Length > 0)
            {
                userData[0].UserName = user.UserName;
                userData[0].BackColor = user.BackColor;
            }
            else
            {
                manager.Add(user);
            }
        }
    }
}
