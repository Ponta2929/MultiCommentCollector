using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MCC.Utility.Binding
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの値が変更されたことを通知します。
        /// </summary>
        /// <param name="propertyName">通知するプロパティ名。</param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        /// <summary>
        /// プロパティに値をセットします。
        /// </summary>
        /// <typeparam name="T">セット先の Type。</typeparam>
        /// <param name="storage">セット先のプロパティ。</param>
        /// <param name="value">セットする値。</param>
        /// <param name="propertyName">通知を行うプロパティ名。</param>
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
