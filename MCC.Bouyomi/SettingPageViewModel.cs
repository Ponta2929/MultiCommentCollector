using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Bouyomi
{
    public class SettingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Setting setting = Setting.Instance;

        public string Format
        {
            get => setting.Format;
            set => Set(ref setting.Format, value);
        }

        public string ApplicationPath
        {
            get => setting.ApplicationPath;
            set => Set(ref setting.ApplicationPath, value);
        }

        public bool Enable
        {
            get => setting.Enable;
            set => Set(ref setting.Enable, value);
        }

        public bool BlackListEnable
        {
            get => setting.BlackListEnable;
            set => Set(ref setting.BlackListEnable, value);
        }

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
