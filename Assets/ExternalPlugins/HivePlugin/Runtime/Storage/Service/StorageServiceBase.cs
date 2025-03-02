using System.IO;
using System.Threading.Tasks;


namespace Modules.Hive.Storage
{
    internal abstract class StorageServiceBase : IStorageService, IAppLifecycleHandler
    {
        public const string RoamingDirectory = "Roaming";
        public const string LocalDirectory = "Local";
        public const string CacheDirectory = "Cache";


        public IDataSourceHub DataSources { get; private set; }

        
        public IDataSourceFactory DataSourceFactory { get; private set; }


        public abstract string GetScopeLocation(StorageScope scope);


        internal virtual Task RunAsync(StorageServiceConfig config)
        {
            DataSources = config.DataSources;
            DataSourceFactory = config.DataSourceFactory;

            return Task.CompletedTask;
        }


        protected static string GetScopeDefaultLocation(string rootPath, StorageScope scope)
        {
            switch (scope)
            {
                case StorageScope.Roaming:
                    return Path.Combine(rootPath, RoamingDirectory);
                case StorageScope.Local:
                    return Path.Combine(rootPath, LocalDirectory);
                case StorageScope.Cache:
                    return Path.Combine(rootPath, CacheDirectory);
                default:
                    throw new System.NotImplementedException($"Unable to determine a root path for storage scope '${scope}'");
            }
        }


        #region App lifecycle handler

        public void OnAppLaunch() { }


        public void OnAppResume() { }


        public void OnAppPause()
        {
            DataSources.SaveAll();
        }


        public void OnAppQuit() { }

        #endregion
    }
}
