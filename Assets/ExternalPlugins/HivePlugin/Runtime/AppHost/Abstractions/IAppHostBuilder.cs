using Modules.Hive.Ioc;
using System.Threading.Tasks;


namespace Modules.Hive
{
    public interface IAppHostBuilder
    {
        IServiceContainer ServiceContainer { get; }

        IAppHostBuilder SetServiceContainer(IServiceContainer serviceContainer);
        IAppHostBuilder AddAppLifecycleHandler(IAppLifecycleHandler appLifecycleHandler);
        IAppHostBuilder AddPlugin(IAppHostPlugin appHostPlugin);
        T GetPlugin<T>() where T : IAppHostPlugin;

        IAppHost Build();
    }

    
    public static class AppHostBuilderExtension
    {
        public static Task ApplyUnityExceptionLogger(this Task task)
        {
            return task.ContinueWith(t =>
            {
                UnityEngine.Debug.Log($"Thread Id = {System.Threading.Thread.CurrentThread.ManagedThreadId}");

                if (t.IsFaulted)
                {
                    UnityEngine.Debug.LogException(t.Exception);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        

        public static Task BuildAndRunAsync(this IAppHostBuilder appHostBuilder)
        {
            IAppHost appHost = appHostBuilder.Build();
            return appHost.RunAsync()
                .ApplyUnityExceptionLogger();
        }
        

        public static Task BuildAndRunAsync(this IAppHostBuilder appHostBuilder, IAppLifecycleHandler appLifecycleHandler)
        {
            appHostBuilder.AddAppLifecycleHandler(appLifecycleHandler);
            IAppHost appHost = appHostBuilder.Build();
            return appHost.RunAsync()
                .ApplyUnityExceptionLogger();
        }
    }
}
