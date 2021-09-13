using MCC.Utility.Binding;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MCC.Bouyomi
{
    public class SettingPageViewModel : BindableBase
    {
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
    }
}
