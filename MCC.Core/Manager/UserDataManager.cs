using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Core.Manager
{
    public class UserDataManager : ListManagerBase<UserData>
    {
        #region Singleton

        private static UserDataManager instance;
        public static UserDataManager Instance => instance ??= new();

        #endregion

        public UserDataManager()
        {
            IsLimit.Value = false;
        }
    }
}
