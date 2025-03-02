using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;


namespace Modules.General.InitializationQueue.Sample
{
    public interface IServiceA
    {
        void Test();
    }

    [InitQueueService(10, bindTo: typeof(IServiceA))]
    public class ServiceA : IInitializableService, IServiceA
    {
        public ServiceA()
        {
            CustomDebug.Log($"ServiceA::ctor");
        }


        public void InitializeService(
            IServiceContainer container,
            Action onComplete,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            CustomDebug.Log("ServiceA:: register");
            onComplete.Invoke();
        }


        public void Test()
        {
            CustomDebug.Log("ServiceA::Test");
        }
    }

    [InitQueueService(10, bindTo: typeof(IServiceA))]
    public class ServiceAMock : IInitializableService, IServiceA
    {
        public void InitializeService(
            IServiceContainer container,
            Action onComplete,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            onComplete?.Invoke();
        }


        public void Test()
        {
            CustomDebug.Log("ServiceAMock::Test");
        }
    }
}
