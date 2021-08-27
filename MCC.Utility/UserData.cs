using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MCC.Utility
{
    [Serializable]
    public class UserData : INotifyPropertyChanged
    {
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
            this.LiveName = data.LiveName;
            this.UserID = data.UserID;
            this.UserName = data.UserName;
            this.BackColor = data.BackColor;

            // 保存用
            this.BeforeUserName = data.UserName;
            this.BeforeBackColor = data.BackColor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの値が変更されたことを通知します。
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new(propertyName));

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
