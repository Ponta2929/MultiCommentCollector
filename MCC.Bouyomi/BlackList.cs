using System.Collections.ObjectModel;

namespace MCC.Bouyomi
{
    public class BlackList : ObservableCollection<BlackListItem>
    {
        #region Singleton

        private static BlackList instance;
        public static BlackList Instance => instance ??= new();

        #endregion
    }
}
