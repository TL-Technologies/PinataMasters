#if UNITY_IOS
using Modules.Hive.InteropServices.PInvoke;
using Modules.Hive.Ioc;
using Modules.Hive.Logging;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Modules.Hive
{
    internal class CommonAppHostIos : CommonAppHost
    {
        [DllImport("__Internal")]
        static extern LogLevel hiveAppHost_getLogLevelMask();

        [DllImport("__Internal")]
        static extern void hiveAppHost_setLogLevelMask(LogLevel level);

        [DllImport("__Internal")]
        static extern void hiveAppHost_run();

        [DllImport("__Internal")]
        static extern void hiveAppHost_setMemoryWarningHandler(VoidCallback handler);


        public CommonAppHostIos(
            IServiceContainer serviceContainer,
            IEventAggregator eventAggregator,
            AppLifecycleDispatcher appLifecycleDispatcher,
            AppHostPluginsHub plugins) :
            base(serviceContainer, eventAggregator, appLifecycleDispatcher, plugins) { }
        

        // TODO: need to add an abstraction to parent class
        public LogLevel LogLevel
        {
            get => hiveAppHost_getLogLevelMask();
            set => hiveAppHost_setLogLevelMask(value);
        }
        

        protected override Task RunAsyncImpl()
        {
            // hiveAppHost_setMemoryWarningHandler()
            hiveAppHost_run();

            return Task.CompletedTask;
        }
    }
}
#endif
