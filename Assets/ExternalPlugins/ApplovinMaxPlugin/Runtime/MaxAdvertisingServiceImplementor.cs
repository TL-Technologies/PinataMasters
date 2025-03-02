using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using Modules.General.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_IOS
using System.Reflection;
#endif


namespace Modules.Max
{
    public class MaxAdvertisingServiceImplementor : IAdvertisingService
    {
        #region Fields

        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        private MaxInterstitialModuleImplementor interstitialModuleImplementor;
        private MaxRewardedAdModuleImplementor rewardedAdModuleImplementor;
        private MaxBannerModuleImplementor bannerModuleImplementor;

        #endregion



        #region Properties

        public bool IsAsyncInitializationEnabled => false;
        
        public bool IsSupportedByCurrentPlatform =>
        #if UNITY_EDITOR
            true;
        #elif UNITY_ANDROID
            true;
        #elif UNITY_IOS
            true;
        #else
            false;
        #endif

        public string ServiceName => "max";

        public List<IEventAds> SupportedAdsModules { get; } = new List<IEventAds>();

        #endregion



        #region Class lifecycle

        public MaxAdvertisingServiceImplementor()
        {
            if (!LLMaxSettings.DoesInstanceExist)
            {
                Debug.LogError("[MaxAdvertisingServiceImplementor] Need LLMaxSettings to init Max");
                return;
            }
            
            MaxUtils.Initialize();
            
            if (LLMaxSettings.Instance.IsBannerEnabled && !string.IsNullOrEmpty(LLMaxSettings.Instance.BannerId))
            {
                SupportedAdsModules.Add(bannerModuleImplementor = new MaxBannerModuleImplementor(this));
            }
            if (LLMaxSettings.Instance.IsInterstitialEnabled && !string.IsNullOrEmpty(LLMaxSettings.Instance.InterstitialId))
            {
                SupportedAdsModules.Add(interstitialModuleImplementor = new MaxInterstitialModuleImplementor(this));
            }
            if (LLMaxSettings.Instance.IsRewardedEnabled && !string.IsNullOrEmpty(LLMaxSettings.Instance.RewardedId))
            {
                SupportedAdsModules.Add(rewardedAdModuleImplementor = new MaxRewardedAdModuleImplementor(this));
            }
        }

        #endregion


        #region Methods

        public void PreInitialize() { }


        public void SetUserConsent(bool isConsentAvailable)
        {
            MaxSdk.SetHasUserConsent(isConsentAvailable);
        }


        public void Initialize()
        {
            interstitialModuleImplementor?.LoadInterstitial();
            rewardedAdModuleImplementor?.LoadRewardedAd();
            bannerModuleImplementor?.LoadBanner();
            
            OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
        }
        
        #endregion
    }
}