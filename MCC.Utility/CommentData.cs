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

        /// <summary>
        /// コメント
        /// </summary>
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        /// <summary>
        /// 追加情報
        /// </summary>
        [JsonPropertyName("Additional")]
        public AdditionalData[] Additional { get; set; }
    }
}
