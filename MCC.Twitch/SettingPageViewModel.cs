using MCC.Utility.Binding;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MCC.Twitch
{
    internal class SettingPageViewModel : BindableBase
    {
        private Setting setting = Setting.Instance;

        public string Password
        {
            get => setting.Password;
            set => Set(ref setting.Password, value);
        }
    }
}
