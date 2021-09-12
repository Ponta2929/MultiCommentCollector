using System;

namespace MCC.Core.Manager
{
    [Serializable]
    public class ConnectionManager : ListManagerBase<ConnectionData>
    {
        #region Singleton

        private static ConnectionManager instance;
        public static ConnectionManager Instance => instance ??= new();
        public static void SetInstance(ConnectionManager inst) => instance = inst;

        #endregion

        public ConnectionManager()
        {
            IsLimit.Value = false;
        }
    }
}
