using Modules.General;
using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    internal class BannerController : AdvertisingController<BannerModuleImplementor>
    {
        #region Fields

        private AdvertisingServiceInfo<BannerModuleImplementor> activeBannerServiceInfo;

        #endregion



        #region Methods

        public bool IsBannerAvailable(string placementName, 
            out AdvertisingServiceInfo<BannerModuleImplementor> availableService)
        {
            bool result = false;
            availableService = null;

            foreach (AdvertisingServiceInfo<BannerModuleImplementor> advertisingServiceInfo in advertisingServiceInfos)
            {
                result = advertisingServiceInfo.controlledElement.IsBannerAvailable;
                
                analyticsTracker.SendAdAvailabilityCheck(
                        advertisingServiceInfo.advertisingService,
                        advertisingServiceInfo.controlledElement.AdModule,
                        result,
                        placementName,
                        string.Empty);

                if (result)
                {
                    availableService = advertisingServiceInfo;
                    break;
                }
            }
            
            return result;
        }
        

        public void ShowBanner(string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            if (!string.IsNullOrEmpty(adShowingPlacement))
            {
                AdvertisingPlacement = adShowingPlacement;
            }
            
            ShowBanner(callback);
        }


        public void ShowBanner(Action<AdActionResultType> callback = null)
        {
            advertisingHideCallback = callback;

            if (IsShowing)
            {
                callback?.Invoke(AdActionResultType.Success);
            }
            else
            {
                if (advertisingServiceInfos.Count > 0)
                {
                    if (IsBannerAvailable(advertisingPlacement, out var bannerServiceInfo))
                    {
                        activeBannerServiceInfo = bannerServiceInfo;
                        IsShowing = true;
                        bannerServiceInfo.controlledElement.ShowBanner(advertisingPlacement);
                    }
                    else
                    {
                        callback?.Invoke(AdActionResultType.NotAvailable);
                    }
                }
                else
                {
                    callback?.Invoke(AdActionResultType.NotAvailable);
                }
            }
        }


        public void HideBanner(Action<AdActionResultType> callback = null)
        {
            if (IsShowing)
            {
                if (activeBannerServiceInfo != null)
                {
                    IsShowing = false;
                    activeBannerServiceInfo.controlledElement.HideBanner();
                    activeBannerServiceInfo = null;

                    callback?.Invoke(AdActionResultType.Success);
                }
                else
                {
                    callback?.Invoke(AdActionResultType.Error);

                    CustomDebug.Log("No active banner showing");
                }
            }
            else
            {
                callback?.Invoke(AdActionResultType.Success);
            }
        }
        
        #endregion



        #region Events handlers

        protected override void AdvertisingService_OnAdShow(IAdvertisingService service, AdModule adModule, AdActionResultType responseResultType,
            int delay, string errorDescription, string adIdentifier)
        {
            base.AdvertisingService_OnAdShow(
                service, 
                adModule, 
                responseResultType, 
                delay, 
                errorDescription, 
                adIdentifier);
            
            advertisingHideCallback?.Invoke(responseResultType);
            advertisingHideCallback = null;
        }

        #endregion
    }
}
