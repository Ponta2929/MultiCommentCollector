using System;
using System.Text.Json.Serialization;

namespace MCC.Utility
{
    [Serializable]
    public class CommentData : PostHeader
    {
        /// <summary>
        /// 投稿時間
        /// </summary>
        [JsonPropertyName("PostTime")]
        public DateTime PostTime { get; set; }

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
        /// コメント
        /// </summary>
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }
    }
}
