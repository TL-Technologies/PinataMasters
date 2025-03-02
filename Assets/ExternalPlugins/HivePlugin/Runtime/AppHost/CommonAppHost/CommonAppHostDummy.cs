using System.Threading.Tasks;
using Modules.Hive.Ioc;


namespace Modules.Hive
{
    internal class CommonAppHostDummy : CommonAppHost
    {
        public CommonAppHostDummy(
            IServiceContainer serviceContainer, 
            IEventAggregator eventAggregator, 
            AppLifecycleDispatcher appLifecycleDispatcher, 
            AppHostPluginsHub plugins) : 
            base(serviceContainer, eventAggregator, appLifecycleDispatcher, plugins) { }
        

        protected override Task RunAsyncImpl()
        {
            return Task.CompletedTask;
        }
    }
}
