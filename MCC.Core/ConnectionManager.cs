namespace MCC.Core
{
    public class ConnectionManager : ListManagerBase<ConnectionData>
    {
        #region Singleton

        private static ConnectionManager instance;
        public static ConnectionManager GetInstance() => instance ??= new();
        public static void SetInstance(ConnectionManager inst) => instance = inst;

        #endregion

        public ConnectionManager()
        {
            IsLimit.Value = false;
        }
    }
}
