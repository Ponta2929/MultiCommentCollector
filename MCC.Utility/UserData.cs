using MCC.Utility.Binding;
using System;
using System.Text.Json.Serialization;

namespace MCC.Utility
{
    [Serializable]
    public class UserData : BindableBase
    {
        /// <summary>
        /// 配信サイト名
        /// </summary>
        [JsonPropertyName("LiveName")]
        public string LiveName { get; set; }

        private bool hideUser;
        /// <summary>
        /// NG
        /// </summary>
        [JsonPropertyName("HideUser")]
        public bool HideUser { get => hideUser; set => Set(ref hideUser, value); }

        private string userName;
        /// <summary>
        /// ユーザー名
        /// </summary>
        [JsonPropertyName("UserName")]
        public string UserName { get => userName; set => Set(ref userName, value); }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [JsonPropertyName("UserID")]
        public string UserID { get; set; }

        private ColorData backColor;
        /// <summary>
        /// 背景色
        /// </summary>
        [JsonPropertyName("BackColor")]
        public ColorData BackColor { get => backColor; set => Set(ref backColor, value); }

        private string beforeUserName;

        public string BeforeUserName { get => beforeUserName; set => Set(ref beforeUserName, value); }

        private ColorData beforeBackColor;

        public ColorData BeforeBackColor { get => beforeBackColor; set => Set(ref beforeBackColor, value); }


        public UserData() { }

        public UserData(CommentDataEx data)
        {
            this.HideUser = false;
            this.LiveName = data.LiveName;
            this.UserID = data.UserID;
            this.UserName = data.UserName;
            this.BackColor = data.BackColor;

            // 保存用
            this.BeforeUserName = data.UserName;
            this.BeforeBackColor = data.BackColor;
        }

        public override bool Equals(object obj)
        {
            var user = obj as UserData;

            if (user is null)
            {
                return false;
            }

            return user.LiveName == LiveName && user.UserID == UserID;
        }

        public override int GetHashCode()
            => LiveName.GetHashCode() ^ UserID.GetHashCode();
    }
}
