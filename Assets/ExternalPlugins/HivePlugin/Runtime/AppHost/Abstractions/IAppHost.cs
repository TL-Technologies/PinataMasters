using Modules.Hive.Ioc;
using Modules.Hive.Storage;
using System.Threading.Tasks;


namespace Modules.Hive
{
    public interface IAppHost
    {
        IServiceContainer ServiceContainer { get; }
        IEventAggregator EventAggregator { get; }
        IAppLifecycleDispatcher AppLifecycleDispatcher { get; }
        IStorageService StorageService { get; }

        Task RunAsync();
    }
}
