using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class EditorAdvertisingServiceImplementor : IAdvertisingService
    {
        #region Fields

        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        private AdvertisingEditorSettings advertisingEditorSettings = default;

        #endregion



        #region Properties

        public bool IsAsyncInitializationEnabled => true;


        public bool IsSupportedByCurrentPlatform =>
            #if UNITY_EDITOR
                true;
            #elif UNITY_ANDROID
                false;
            #elif UNITY_IOS
                false;
            #else
                false;
            #endif


        public string ServiceName => "editor";


        public List<IEventAds> SupportedAdsModules { get; }

        #endregion



        #region Class lifecycle

        public EditorAdvertisingServiceImplementor(AdvertisingEditorSettings settings)
        {
            advertisingEditorSettings = settings;
            SupportedAdsModules = new List<IEventAds>();

            foreach (AdModule module in advertisingEditorSettings.SupportedModules)
            {
                switch (module)
                {
                    case AdModule.Interstitial:
                        SupportedAdsModules.Add(new AdvertisingEditorInterstitialModuleImplementor(this, settings));
                        break;
                    case AdModule.RewardedVideo:
                        SupportedAdsModules.Add(new AdvertisingEditorRewardedVideoModuleImplementor(this, settings));
                        break;
                    case AdModule.Banner:
                        SupportedAdsModules.Add(new AdvertisingEditorBannerModuleImplementor(this, settings));
                        break;
                    default:
                        throw new NotImplementedException($"Module {module} not implemented in MoPub");
                }
            }
        }

        #endregion



        #region Methods

        public void PreInitialize() { }


        public void SetUserConsent(bool isConsentAvailable) { }


        public void Initialize()
        {
            OnServiceInitialized?.Invoke(this, InitializationStatus.Success);

            foreach (IEventAds supportedAdsModule in SupportedAdsModules)
            {
                (supportedAdsModule as IAdsInitializer)?.Initialize();
            }
        }

        #endregion
    }
}