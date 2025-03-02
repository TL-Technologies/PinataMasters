using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;


namespace Modules.General.InitializationQueue
{
    public interface IInitializableService
    {
        // "Factory" method to register current service into provided container
        void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback);
    }
}
