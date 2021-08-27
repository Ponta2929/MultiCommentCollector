using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCC.Utility
{
    [Serializable]
    public class CommentDataEx : CommentData
    {
        private ColorData backColor;

        [JsonPropertyName("BackColor")]
        public ColorData BackColor { get => backColor; set => Set(ref backColor, value); }

        public CommentDataEx() { }

        public CommentDataEx(CommentData data)
        {
            this.Comment = data.Comment;
            this.LiveName = data.LiveName;
            this.PostTime = data.PostTime;
            this.PostType = data.PostType;
            this.UserID = data.UserID;
            this.UserName = data.UserName;
            this.BackColor = ColorData.FromArgb(0, 255, 255, 255);
        }

        public void SetUserData(UserData userData)
        {
            this.BackColor = userData.BackColor;
            this.UserName = userData.UserName;
        }
    }
}
