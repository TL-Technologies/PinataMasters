using System;


namespace Modules.General.ServicesInitialization
{
    public interface IInitializable
    {
        event Action<IInitializable, InitializationStatus> OnServiceInitialized;
        
        bool IsAsyncInitializationEnabled { get; }

        void PreInitialize();
        void SetUserConsent(bool isConsentAvailable);
        void Initialize();
    }
}
