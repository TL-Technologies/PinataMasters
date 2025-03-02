using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    internal class InterstitialController : AdvertisingController<InterstitialModuleImplementor>
    {
        #region Methods

        public bool IsInterstitialAvailable(out AdvertisingServiceInfo<InterstitialModuleImplementor> availableService)
        {
            bool result = false;
            availableService = null;

            foreach (AdvertisingServiceInfo<InterstitialModuleImplementor> advertisingServiceInfo in advertisingServiceInfos)
            {
                result = advertisingServiceInfo.controlledElement.IsInterstitialAvailable;
                availableService = advertisingServiceInfo;
                
                if (result)
                {
                    break;
                }
            }
            
            return result;
        }
        
        
        public void SendAdAvailabilityCheck(string adShowingPlacement, bool isAdAvailable, AdvertisingServiceInfo<InterstitialModuleImplementor> serviceInfo, string errorDescription)
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


        public void ShowInterstitial(string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            bool isInterAvailable = IsInterstitialAvailable(out var interstitialServiceInfo);
            if (IsShowing)
            {
                SendAdAvailabilityCheck(adShowingPlacement, false, interstitialServiceInfo, AdAvailabilityErrorType.AdIsShowing);
                callback?.Invoke(AdActionResultType.NotAvailable);
            }
            else if (advertisingServiceInfos.Count == 0)
            {
                SendAdAvailabilityCheck(adShowingPlacement, false, interstitialServiceInfo, AdAvailabilityErrorType.NoAdServiceAvailable);
                callback?.Invoke(AdActionResultType.NotAvailable);
            }
            else
            {
                advertisingHideCallback = callback;
                advertisingPlacement = adShowingPlacement;
                
                if (isInterAvailable)
                {
                    IsShowing = true;
                    SendAdAvailabilityCheck(adShowingPlacement, true, interstitialServiceInfo, string.Empty);
                    interstitialServiceInfo.controlledElement.ShowInterstitial(adShowingPlacement);
                }
                else
                {
                    SendAdAvailabilityCheck(adShowingPlacement, false, interstitialServiceInfo, AdAvailabilityErrorType.NoAdAvailable);
                    callback?.Invoke(AdActionResultType.NotAvailable);
                }
            }
        }


        public void HideInterstitial(Action<AdActionResultType> callback = null)
        {
            IsShowing = false;
            callback?.Invoke(AdActionResultType.Error);
            
            throw new NotImplementedException(
                "HideInterstitial method for IInterstitial not implemented");
        }

        #endregion
    }
}
