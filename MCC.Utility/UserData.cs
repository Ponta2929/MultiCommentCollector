using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MCC.Utility
{
    [Serializable]
    public class UserData
    {
        /// <summary>
        /// 配信サイト名
        /// </summary>
        [JsonPropertyName("LiveName")]
        public string LiveName { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [JsonPropertyName("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        [JsonPropertyName("BackColor")]
        public ColorData BackColor { get; set; }

        public UserData() { }

        public UserData(CommentDataEx data)
        {
            this.LiveName = data.LiveName;
            this.UserID = data.UserID;
            this.UserName = data.UserName;
            this.BackColor = data.BackColor;
        }
    }
}
