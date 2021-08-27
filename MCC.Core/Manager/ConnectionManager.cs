namespace MCC.Core.Manager
{
    public class ConnectionManager : ListManagerBase<ConnectionData>
    {
        #region Singleton

        private static ConnectionManager instance;
        public static ConnectionManager Instance => instance ??= new();

        #endregion

        public ConnectionManager()
        {
            IsLimit.Value = false;
        }
    }
}
