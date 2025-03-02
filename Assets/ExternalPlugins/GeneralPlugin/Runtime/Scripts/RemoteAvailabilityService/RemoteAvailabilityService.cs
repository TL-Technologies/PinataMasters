using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;


namespace Modules.General
{
    [InitQueueService(3)]
    public class RemoteAvailabilityService : IInitializableService
    {
        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            var abTest = container.GetService<IAbTestService>();
            if (abTest != null)
            {
                var remoteAvailabilityData = abTest.GetTestData<RemoteAvailabilityAbTestData>();
                if (!remoteAvailabilityData.IsApplicationEnabled)
                {
                    var popupService = Services.SystemPopupService;
                    popupService?.ShowPopupWithoutButtons("Error", "Sorry, this game is unavailable in your country");
                    CustomDebug.LogError("Application is not allowed to run");
                    onErrorCallback.Invoke(this, InitializationStatus.Failed);
                    return;
                }
            }

            CustomDebug.Log("Application is allowed to run");
            onCompleteCallback.Invoke();
        }
    }
}
