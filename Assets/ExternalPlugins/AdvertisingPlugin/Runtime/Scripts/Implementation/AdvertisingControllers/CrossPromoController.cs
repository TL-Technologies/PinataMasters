using Modules.General.Abstraction;
using System;
using UnityEngine;


namespace Modules.Advertising
{
    internal class CrossPromoController : AdvertisingController<CrossPromoModuleImplementor>
    {
        #region Properties

        public int FirstInterstitialShownDataInSeconds
        {
            get => PlayerPrefs.GetInt("interstitialFirstShown", -1);
            private set => PlayerPrefs.SetInt("interstitialFirstShown", value);
        }


        public int CountInterstitialShownPerDay
        {
            get => PlayerPrefs.GetInt("interstitialTimesShown", 0);
            private set => PlayerPrefs.SetInt("interstitialTimesShown", value);
        }

        #endregion
        
        
        
        #region Methods
        
        public bool IsInterstitialAvailable(string placementName, 
            out AdvertisingServiceInfo<CrossPromoModuleImplementor> availableService)
        {
            bool result = false;
            availableService = null;

            UpdateInterstitialDailyData();

            foreach (AdvertisingServiceInfo<CrossPromoModuleImplementor> advertisingServiceInfo in advertisingServiceInfos)
            {
                if (advertisingServiceInfo.controlledElement.IsInterstitialAvailable)
                {
                    result = true;
                    availableService = advertisingServiceInfo;
                    break;
                }
            }
            
            return result;
        }
        

        public void ShowInterstitial(string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            if (!IsShowing && advertisingServiceInfos.Count > 0)
            {
                advertisingHideCallback = callback;
                advertisingPlacement = adShowingPlacement;

                if (IsInterstitialAvailable(adShowingPlacement, out var interstitialServiceInfo))
                {
                    CountInterstitialShownPerDay++;
                    IsShowing = true;
                    interstitialServiceInfo.controlledElement.ShowInterstitial();
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


        public void UpdateInterstitialDailyData()
        {
            int timeNow = (int) new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            if (timeNow - FirstInterstitialShownDataInSeconds > 86400)
            {
                CountInterstitialShownPerDay = 0;
            }
        }
        
        
        public void SetSubscriptionPopupActive(bool isActive)
        {
            foreach (AdvertisingServiceInfo<CrossPromoModuleImplementor> advertisingServiceInfo in advertisingServiceInfos)
            {
                advertisingServiceInfo.controlledElement.IsSubscriptionPopupActive = isActive;
            }
        }

        #endregion



        #region Events handlers

        protected override void AdvertisingService_OnAdShow(IAdvertisingService service, AdModule adModule, AdActionResultType responseResultType,
            int delay, string errorDescription, string adIdentifier)
        {
            base.AdvertisingService_OnAdShow(service, adModule, responseResultType, delay, errorDescription, adIdentifier);
            
            if (CountInterstitialShownPerDay == 1)
            {
                FirstInterstitialShownDataInSeconds = (int) new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            }
        }

        #endregion
    }
}
