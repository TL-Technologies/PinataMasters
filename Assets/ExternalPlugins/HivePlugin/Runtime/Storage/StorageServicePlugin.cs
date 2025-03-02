using Modules.Hive.Ioc;
using System.Threading.Tasks;


namespace Modules.Hive.Storage
{
    internal class StorageServicePlugin : IAppHostPlugin
    {
        public AppHostLayer Layer => AppHostLayer.PostInternal;


        public StorageServiceConfig Config { get; }


        #region Instancing and initialization

        public StorageServicePlugin(StorageServiceConfig config)
        {
            Config = config;
        }


        public void Setup(IAppHostBuilder appHostBuilder) { }


        public Task ConfigureAsync(IAppHost appHost)
        {
            #if UNITY_EDITOR
                appHost.ServiceContainer.AddSingletonService<StorageServiceEditor, IStorageService>(ServiceBindingOptions.Exclusive);
            #elif UNITY_ANDROID
                appHost.ServiceContainer.AddSingletonService<StorageServiceAndroid, IStorageService>(ServiceBindingOptions.Exclusive);
            #else
                appHost.ServiceContainer.AddSingletonService<StorageServiceGeneric, IStorageService>(ServiceBindingOptions.Exclusive);
            #endif

            return Task.CompletedTask;
        }


        public Task RunAsync(IAppHost appHost)
        {
            StorageServiceBase storageManager = (StorageServiceBase)appHost.ServiceContainer.GetRequiredService<IStorageService>();
            appHost.AppLifecycleDispatcher.Add(storageManager, Layer, -1000);
            return storageManager.RunAsync(Config);
        }

        #endregion
    }
}
