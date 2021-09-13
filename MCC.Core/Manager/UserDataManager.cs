using MCC.Utility;
using System;

namespace MCC.Core.Manager
{
    [Serializable]
    public class UserDataManager : ListManagerBase<UserData>
    {
        #region Singleton

        private static UserDataManager instance;
        public static UserDataManager Instance => instance ??= new();
        public static void SetInstance(UserDataManager inst) => instance = inst;

        #endregion

        public UserDataManager() => IsLimit.Value = false;
    }
}
