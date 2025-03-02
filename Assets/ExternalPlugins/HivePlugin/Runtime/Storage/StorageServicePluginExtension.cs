using Modules.Hive.Storage;
using System;


namespace Modules.Hive
{
    public static class StorageServicePluginExtension
    {
        public static IAppHostBuilder AddStorageService(this IAppHostBuilder appHostBuilder)
        {
            StorageServiceConfig config = new StorageServiceConfig();
            StorageServicePlugin plugin = new StorageServicePlugin(config);

            // Register plugin
            appHostBuilder.AddPlugin(plugin);
            return appHostBuilder;
        }


        public static IAppHostBuilder ConfigureStorageService(this IAppHostBuilder appHostBuilder, Action<StorageServiceConfig> action)
        {
            StorageServicePlugin plugin = appHostBuilder.GetPlugin<StorageServicePlugin>();
            action(plugin.Config);

            return appHostBuilder;
        }
    }
}
