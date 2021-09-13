using System.Text.Json.Serialization;

namespace MCC.Bouyomi
{

    public class BlackListItem
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
        /// コメント
        /// </summary>
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        public BlackListItem()
        {
            LiveName = "*";
            UserName = "*";
            UserID = "*";
            Comment = "*";
        }
    }
}
