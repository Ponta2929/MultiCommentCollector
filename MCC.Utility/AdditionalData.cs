using MCC.Utility.Binding;
using System.Text.Json.Serialization;

namespace MCC.Utility
{
    public class AdditionalData : BindableBase
    {
        /// <summary>
        /// 投稿時間
        /// </summary>
        [JsonPropertyName("Enable")]
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 配信サイト名
        /// </summary>
        [JsonPropertyName("Data")]
        public string Data { get; set; }

        /// <summary>
        /// 配信サイト名
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }
    }
}
