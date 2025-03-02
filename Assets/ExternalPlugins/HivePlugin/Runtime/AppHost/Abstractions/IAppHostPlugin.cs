using System.Threading.Tasks;


namespace Modules.Hive
{
    public interface IAppHostPlugin
    {
        AppHostLayer Layer { get; }

        void Setup(IAppHostBuilder appHostBuilder);
        Task ConfigureAsync(IAppHost appHost);
        Task RunAsync(IAppHost appHost);
    }
}
