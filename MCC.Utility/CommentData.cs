using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MCC.Utility
{
    [Serializable]
    public class CommentData : PostHeader, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの値が変更されたことを通知します。
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new(propertyName));

        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;

            // イベント
            OnPropertyChanged(propertyName);
        }
    }
}
