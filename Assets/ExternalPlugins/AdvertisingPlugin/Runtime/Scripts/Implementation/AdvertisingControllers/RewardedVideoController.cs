using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    internal class RewardedVideoController : AdvertisingController<RewardedVideoModuleImplementor>
    {
        #region Methods

        public bool IsRewardedVideoAvailable(out AdvertisingServiceInfo<RewardedVideoModuleImplementor> availableService)
        {
            bool result = false;
            availableService = null;

            foreach (AdvertisingServiceInfo<RewardedVideoModuleImplementor> advertisingServiceInfo in
                advertisingServiceInfos)
            {
                result = advertisingServiceInfo.controlledElement.IsVideoAvailable;
                availableService = advertisingServiceInfo;
                
                if (result)
                {
                    break;
                }
            }

            return result;
        }
        
        
        public void SendAdAvailabilityCheck(string adShowingPlacement, bool isAdAvailable, AdvertisingServiceInfo<RewardedVideoModuleImplementor> serviceInfo, string errorDescription)
        {
            if (serviceInfo == null)
            {
                return;
            }
            
            analyticsTracker.SendAdAvailabilityCheck(
                serviceInfo.advertisingService,
                serviceInfo.controlledElement.AdModule,
                isAdAvailable,
                adShowingPlacement,
                errorDescription);
        }


        public void ShowVideo(string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            bool isVideoAvailable = IsRewardedVideoAvailable(out var rewardedVideoServiceInfo);
            if (IsShowing)
            {
                SendAdAvailabilityCheck(adShowingPlacement, false, rewardedVideoServiceInfo, AdAvailabilityErrorType.AdIsShowing);
                callback?.Invoke(AdActionResultType.NotAvailable);
            }
            else if (advertisingServiceInfos.Count == 0)
            {
                SendAdAvailabilityCheck(adShowingPlacement, false, rewardedVideoServiceInfo, AdAvailabilityErrorType.NoAdServiceAvailable);
                callback?.Invoke(AdActionResultType.NotAvailable);
            }
            else
            {
                advertisingHideCallback = callback;
                advertisingPlacement = adShowingPlacement;
                
                if (isVideoAvailable)
                {
                    IsShowing = true;
                    SendAdAvailabilityCheck(adShowingPlacement, true, rewardedVideoServiceInfo, string.Empty);
                    rewardedVideoServiceInfo.controlledElement.ShowVideo(adShowingPlacement);
                }
                else
                {
                    SendAdAvailabilityCheck(adShowingPlacement, false, rewardedVideoServiceInfo, AdAvailabilityErrorType.NoAdAvailable);
                    callback?.Invoke(AdActionResultType.NotAvailable);
                }
            }
        }


        public void HideRewardedVideo(Action<AdActionResultType> callback = null)
        {
            IsShowing = false;
            callback?.Invoke(AdActionResultType.Error);
            
            throw new NotImplementedException(
                "HideRewardedVideo method for IRewardedVideo not implemented");
        }

        #endregion
    }
}