using Modules.General;
using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    [InitQueueService(4, bindTo: typeof(IAdvertisingManager))]
    public partial class AdvertisingManager : BaseAdvertisingManager, IAdvertisingManager, IInitializableService
    {
        #region Fields

        private DateTime lastInterstitialShowDateTime = DateTime.MinValue;

        private IAdvertisingNecessaryInfo advertisingNecessaryInfo;
        private IInGameAdvertisingAbTestData inGameAdvertisingAbTestData;
        private AdsRewardedVideoPlacementsController adsRewardedVideoPlacementsController;

        private ICrossPromoAdvertisingAbTestData crossPromoAdvertisingAbTestData;

        private int interstitialTimesShown;

        private bool isSubscriptionShowing;

        private ISoundManager soundManager;

        private static IAdvertisingManager instance;

        #endregion



        #region Properties

        public new static IAdvertisingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Services.GetService<IAdvertisingManager>();
                }

                return instance;
            }
        }


        public bool IsSubscriptionShowing
        {
            get => isSubscriptionShowing;
            set
            {
                if (isSubscriptionShowing != value)
                {
                    isSubscriptionShowing = value;

                    crossPromoController.SetSubscriptionPopupActive(isSubscriptionShowing);
                    UpdateBannerVisibility(bannerController.AdvertisingPlacement);
                }
            }
        }


        protected IInGameAdvertisingAbTestData InGameAdvertisingAbTestData
        {
            get
            {
                if (inGameAdvertisingAbTestData == null)
                {
                    throw new NotImplementedException("Empty value for inGameAdvertisingAbTestData.");
                }

                return inGameAdvertisingAbTestData;
            }
        }


        private IAdvertisingNecessaryInfo AdvertisingNecessaryInfo
        {
            get
            {
                if (advertisingNecessaryInfo == null)
                {
                    throw new NotImplementedException("Empty value for advertisingNecessaryInfo. " +
                                                      "You should call Initialize before use them.");
                }

                return advertisingNecessaryInfo;
            }
        }


        private ICrossPromoAdvertisingAbTestData CrossPromoAdvertisingAbTestData
        {
            get
            {
                if (crossPromoAdvertisingAbTestData == null)
                {
                    throw new NotImplementedException("Empty value for advertisingAbTestData.");
                }

                return crossPromoAdvertisingAbTestData;
            }
        }


        private ISoundManager SoundManager => soundManager ?? (soundManager = Services.SoundManager);


        private float InactivityTime => InactivityTimer.HasFoundInstance ? InactivityTimer.Instance.InactivityTime : 0;


        private bool IsInterstitialDelayEnded => (DateTime.Now - lastInterstitialShowDateTime).TotalSeconds >=
                                                 InGameAdvertisingAbTestData.delayBetweenInterstitials;

        #endregion



        #region Methods

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            var settings = container.GetService<IAdvertisingManagerSettings>();
            if (settings != null)
            {
                OnInitialized += OnManagerInitialized;

                Initialize(settings.AdServices,
                           settings.AdvertisingInfo,
                           settings.AbTestData,
                           settings.AdAvailabilityParameters,
                           settings.AdvertisingPlacements);
            }
            else
            {
                onErrorCallback?.Invoke(this, InitializationStatus.Failed);
            }

            void OnManagerInitialized(InitializationStatus status)
            {
                OnInitialized -= OnManagerInitialized;

                switch (status)
                {
                    case InitializationStatus.Failed:
                        onErrorCallback?.Invoke(this, InitializationStatus.Failed);
                        break;
                    case InitializationStatus.None:
                    case InitializationStatus.Success:
                        onCompleteCallback?.Invoke();
                        break;
                }
            }
        }


        public virtual void Initialize(
            IAdvertisingService[] adServices,
            IAdvertisingNecessaryInfo advertisingInfo,
            IAdvertisingAbTestData[] abTestData,
            List<AdAvailabilityParameter> adAvailabilityParameters = null,
            List<AdPlacementModel> advertisingPlacements = null)
        {
            LLApplicationStateRegister.OnApplicationEnteredBackground +=
                LLApplicationStateRegister_OnApplicationEnteredBackground;

            InitializeNecessaryInfo(advertisingInfo);

            InitializeAbTestData(abTestData);
            
            base.Initialize(adServices, adAvailabilityParameters, advertisingPlacements);
        }
        

        public override void TryShowAdByModule(AdModule module, string adShowingPlacement,
                                               Action<AdActionResultType> callback = null)
        {
            if (module == AdModule.Interstitial)
            {
                if (IsCrossPromoAvailable)
                {
                    if (IsAdModuleByPlacementAvailable(module, adShowingPlacement))
                    {
                        bannerController.HideBanner();
                        crossPromoController.ShowInterstitial(adShowingPlacement, callback);
                    }
                    else
                    {
                        callback?.Invoke(AdActionResultType.NotAvailable);
                    }
                }
                else
                {
                    if (IsInterstitialDelayEnded)
                    {
                        base.TryShowAdByModule(module, adShowingPlacement, callback);
                    }
                    else
                    {
                        callback?.Invoke(AdActionResultType.NotAvailable);
                    }
                }
            }
            else
            {
                base.TryShowAdByModule(module, adShowingPlacement, callback);
            }
        }


        public RewardedVideoPlacementSettings GetPlacementSettings(string placement)
        {
            return adsRewardedVideoPlacementsController.GetPlacementSettings(placement,
                AdvertisingNecessaryInfo.CurrentPlayerLevel);
        }

        private void InitializeNecessaryInfo(IAdvertisingNecessaryInfo advertisingInfo)
        {
            advertisingNecessaryInfo = advertisingInfo;
            advertisingNecessaryInfo.OnPersonalDataDeletingDetect +=
                AdvertisingNecessaryInfo_OnPersonalDataDeletingDetect;
            advertisingNecessaryInfo.OnPurchasesListUpdate += AdvertisingNecessaryInfo_OnPurchasesListUpdate;
            advertisingNecessaryInfo.OnPlayerLevelUpdate += AdvertisingNecessaryInfo_OnPlayerLevelUpdate;
        }

        private void InitializeAbTestData(IAdvertisingAbTestData[] abTestData)
        {
            Dictionary<string, AbTestRvSettings> rvSettings = new Dictionary<string, AbTestRvSettings>();

            foreach (IAdvertisingAbTestData testData in abTestData)
            {
                if (testData is IInGameAdvertisingAbTestData gameAdvertisingAbTestData)
                {
                    inGameAdvertisingAbTestData = gameAdvertisingAbTestData;
                }

                if (testData is IAbTestRewardSettingsData abTestRewardSettingsData)
                {
                    foreach (RvPlacementAbTestData rewardSetting in abTestRewardSettingsData.GetRewardsSettings())
                    {
                        rvSettings.Add(rewardSetting.PlacementName, rewardSetting.RvSettings);
                    }
                }

                if (testData is ICrossPromoAdvertisingAbTestData crossPromoAbTestData)
                {
                    crossPromoAdvertisingAbTestData = crossPromoAbTestData;
                }
            }

            adsRewardedVideoPlacementsController =
                new AdsRewardedVideoPlacementsController(rvSettings);
        }
        
        
        public void CreateInactivityTimer(Func<bool> condition = null)
        {
            InactivityTimer.Instance.OnUpdateInactivityTimer += ActivityUpdater_OnUpdateInactivityTimer;
            InactivityTimer.Instance.UpdateTimerCondition = condition;
        }
        
        #endregion



        #region Events handlers

        protected override void Controller_OnAdShow(
            AdModule adModule,
            AdActionResultType responseResultType,
            int delay,
            string errorDescription,
            string adIdentifier,
            string advertisingPlacement)
        {
            base.Controller_OnAdShow(adModule, responseResultType, delay, errorDescription, adIdentifier, advertisingPlacement);

            if (responseResultType == AdActionResultType.Success)
            {
                if (adModule == AdModule.Interstitial)
                {
                    interstitialTimesShown++;
                }

                if (adModule == AdModule.Interstitial || adModule == AdModule.RewardedVideo)
                {
                    SoundManager?.SetBlocked(true, typeof(AdvertisingManager).ToString());
                }
            }
        }


        protected override void Controller_OnAdHide(
            AdModule adModule,
            AdActionResultType responseResultType,
            string errorDescription,
            string adIdentifier)
        {
            base.Controller_OnAdHide(adModule, responseResultType, errorDescription, adIdentifier);

            switch (adModule)
            {
                case AdModule.Interstitial:
                case AdModule.RewardedVideo:
                    // According to technical document.
                    lastInterstitialShowDateTime = DateTime.Now;

                    SoundManager?.SetBlocked(false, typeof(AdvertisingManager).ToString());

                    break;
            }
        }


        protected override void Controller_OnAdRespond(AdModule adModule, int delay, AdActionResultType responseResultType,
            string errorDescription, string adIdentifier)
        {
            base.Controller_OnAdRespond(adModule, delay, responseResultType, errorDescription, adIdentifier);

            if (responseResultType == AdActionResultType.Success)
            {
                switch (adModule)
                {
                    case AdModule.Banner:
                        TryShowAdByModule(AdModule.Banner, AdPlacementType.DefaultPlacement);
                        break;
                    default:
                        break;
                }
            }
        }


        private void AdvertisingNecessaryInfo_OnPersonalDataDeletingDetect()
        {
            TryHideAdByModule(AdModule.Banner);
        }


        private void AdvertisingNecessaryInfo_OnPurchasesListUpdate()
        {
            if (!IsAdModuleByPlacementAvailable(AdModule.Banner, AdPlacementType.DefaultPlacement))
            {
                TryHideAdByModule(AdModule.Banner);
            }
        }


        private void AdvertisingNecessaryInfo_OnPlayerLevelUpdate(int levelNumber)
        {
            TryShowAdByModule(AdModule.Banner, AdPlacementType.DefaultPlacement);
        }


        private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEntered)
        {
            if (!isEntered)
            {
                Scheduler.Instance.CallMethodWithDelay(this, () =>
                {
                    TryShowAdByModule(AdModule.Interstitial, adShowingPlacement: AdPlacementType.Background);
                }, 0.0f);
            }
        }

        private void ActivityUpdater_OnUpdateInactivityTimer()
        {
            if (!IsAdModuleByPlacementAvailable(AdModule.Interstitial, AdPlacementType.Inactivity))
            {
                return;
            }

            TryShowAdByModule(AdModule.Interstitial, AdPlacementType.Inactivity,actionResultType =>
            {
                if (InactivityTimer.HasFoundInstance)
                {
                    InactivityTimer.Instance.ResetInactivityTimer();
                }
            });
        }

        #endregion
    }
}