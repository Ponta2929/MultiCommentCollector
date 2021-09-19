using MCC.Utility.Binding;

namespace MCC.Youtube
{
    internal class SettingPageViewModel : BindableBase
    {
        private Setting setting = Setting.Instance;

        public string Password
        {
            get => setting.APIKey;
            set => Set(ref setting.APIKey, value);
        }
    }
}
