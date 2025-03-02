using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;


namespace Modules.General.InitializationQueue.Sample
{
    public interface IServiceB
    {
        void Test();
    }


    [InitQueueService(1, bindTo: typeof(IServiceB))]
    public class ServiceB : IInitializableService, IServiceB
    {
        public ServiceB()
        {
            CustomDebug.Log("ServiceB::ctor");
        }


        public void InitializeService(
            IServiceContainer container,
            Action onComplete,
            Action<IInitializableService, InitializationStatus> onErrorCallback = null)
        {
            CustomDebug.Log("ServiceB:: register");
            onComplete.Invoke();
        }


        public void Test()
        {
            CustomDebug.Log("ServiceB::Test");
        }
    }
}
