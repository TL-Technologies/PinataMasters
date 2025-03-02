using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAdvertisingManager : IInitializationResultNotifier
    {
        event Action<AdModule, AdActionResultType> OnAdRespond;
        event Action<AdModule, AdActionResultType> OnAdShow;
        event Action<AdModule, AdActionResultType> OnAdHide;
        event Action<AdModule> OnAdClick;

        void Initialize(
            IAdvertisingService[] adServices,
            IAdvertisingNecessaryInfo advertisingInfo,
            IAdvertisingAbTestData[] abTestData,
            List<AdAvailabilityParameter> adAvailabilityParameters = null,
            List<AdPlacementModel> advertisingPlacements = null);
        
        void TryShowAdByModule(AdModule module, string adShowingPlacement, Action<AdActionResultType> callback = null);
        
        void TryHideAdByModule(AdModule module, Action<AdActionResultType> callback = null);
        
        bool IsAdModuleByPlacementAvailable(AdModule module, string placementName);
        
        RewardedVideoPlacementSettings GetPlacementSettings(string placement);

        void LockAd(AdModule adModule, string reason);

        void UnlockAd(AdModule adModule, string reason);
        
        bool IsSubscriptionShowing { get; set; }
        
        T GetService<T>() where T : class, IAdvertisingService;

        bool IsFullScreenAdShowing { get; }

        void CreateInactivityTimer(Func<bool> checkCondition = null);
    }
}
