using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using Modules.Networking;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;


namespace Modules.Advertising
{
    public class BaseAdvertisingManager : ServicesInitializer
    {
        #region Fields

        public event Action<AdModule, AdActionResultType> OnAdRespond;
        public event Action<AdModule> OnAdBeforeShow;
        public event Action<AdModule, AdActionResultType> OnAdShow;
        public event Action<AdModule, AdActionResultType> OnAdHide;
        public event Action<AdModule> OnAdClick;

        protected AdAvailabilityParametersLib availabilityParametersLib;
        protected AdPlacementsController adPlacementsController;
        
        internal readonly AdAnalyticsTracker analyticsTracker;
        internal readonly CrossPromoController crossPromoController;
        internal readonly BannerController bannerController;
        private readonly InterstitialController interstitialController;
        private readonly RewardedVideoController rewardedVideoController;
        private readonly Dictionary<Type, IAdvertisingService> availableAdvertisingServices;

        private bool isNeedShowBanner;

        private static BaseAdvertisingManager instance;
        
        private Dictionary<AdModule, List<string>> lockAdReasons = new Dictionary<AdModule, List<string>>()
        {
            { AdModule.Banner, new List<string>() },
            { AdModule.Interstitial, new List<string>() },
            { AdModule.RewardedVideo, new List<string>() }
        };
        
        #endregion



        #region Properties

        /// <summary>
        /// Gets instance of the BaseAdvertisingManager.
        /// </summary>
        public static BaseAdvertisingManager Instance => instance ?? (instance = new BaseAdvertisingManager());
        
        
        /// <summary>
        /// Property for checking is full screen ads (rewarded video or interstitial) showing.
        /// </summary>
        public bool IsFullScreenAdShowing
        {
            get => interstitialController.IsShowing || 
                   rewardedVideoController.IsShowing || 
                   crossPromoController.IsShowing;
        }


        /// <summary>
        /// Property for checking is banner ads showing.
        /// </summary>
        public bool IsBannerShowing => bannerController.IsShowing;

        #endregion



        #region Class lifecycle

        protected BaseAdvertisingManager()
        {
            availableAdvertisingServices = new Dictionary<Type, IAdvertisingService>();
            analyticsTracker = new AdAnalyticsTracker();
            availabilityParametersLib = new AdAvailabilityParametersLib();
            adPlacementsController = new AdPlacementsController();
            
            interstitialController = new InterstitialController();
            interstitialController.OnAdRespond += Controller_OnAdRespond;
            interstitialController.OnAdShow += Controller_OnAdShow;
            interstitialController.OnAdHide += Controller_OnAdHide;
            interstitialController.OnAdClick += Controller_OnAdClick;

            rewardedVideoController = new RewardedVideoController();
            rewardedVideoController.OnAdRespond += Controller_OnAdRespond;
            rewardedVideoController.OnAdShow += Controller_OnAdShow;
            rewardedVideoController.OnAdHide += Controller_OnAdHide;
            rewardedVideoController.OnAdClick += Controller_OnAdClick;
            
            bannerController = new BannerController();
            bannerController.OnAdRespond += Controller_OnAdRespond;
            bannerController.OnAdShow += Controller_OnAdShow;
            bannerController.OnAdHide += Controller_OnAdHide;
            bannerController.OnAdClick += Controller_OnAdClick;
            
            crossPromoController = new CrossPromoController();
            crossPromoController.OnAdRespond += Controller_OnAdRespond;
            crossPromoController.OnAdShow += Controller_OnAdShow;
            crossPromoController.OnAdHide += Controller_OnAdHide;
            crossPromoController.OnAdClick += Controller_OnAdClick;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Performs service initialization with specified array of services.
        /// </summary>
        /// <param name="adServices">The array of analytics services.</param>
        /// <param name="adAvailabilityParameters">
        /// List of additional advertising placements availability parameters.
        /// </param>
        /// <param name="advertisingPlacements">List of additional advertising placements.</param>
        public void Initialize(
            IAdvertisingService[] adServices,
            List<AdAvailabilityParameter> adAvailabilityParameters = null,
            List<AdPlacementModel> advertisingPlacements = null)
        {
            IAdvertisingService[] supportedAdServices = 
                adServices.Where(service => service.IsSupportedByCurrentPlatform).ToArray();

            interstitialController.Initialize(supportedAdServices, analyticsTracker);
            bannerController.Initialize(supportedAdServices, analyticsTracker);
            rewardedVideoController.Initialize(supportedAdServices, analyticsTracker);
            crossPromoController.Initialize(supportedAdServices, analyticsTracker);
            
            FillPreDefinedAvailabilityParameters();
            FillCustomAdAvailabilityParameters(adAvailabilityParameters);
            FillPreDefinedPlacements();
            FillCustomPlacements(advertisingPlacements);

            base.Initialize(supportedAdServices);
        }
        
        
        /// <summary>
        /// Gets a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public T GetService<T>() where T : class, IAdvertisingService
        {
            T result = null;
            availableAdvertisingServices.TryGetValue(typeof(T), out var resultAnalyticsService);

            if (resultAnalyticsService != null)
            {
                result = resultAnalyticsService as T;
            }
            else
            {
                CustomDebug.LogError($"Service with type {typeof(T)} is empty");
            }
            
            return result;
        }
       
        
        /// <summary>
        /// Check availability of Advertising module.
        /// </summary>
        /// <param name="module">The type of advertising module.</param>
        /// <param name="placement">The placement of advertising.</param>
        /// <returns>True if advertising by module available and not showing.</returns>
        public virtual bool IsAdModuleByPlacementAvailable(AdModule adModule, string placementName)
        {
            switch (adModule)
            {
                case AdModule.Interstitial:
                case AdModule.RewardedVideo:
                case AdModule.Banner:
                    return !IsFullScreenAdShowing && 
                           adPlacementsController.IsAdModuleByPlacementAvailable(adModule, placementName);
                default:
                    CustomDebug.LogError($"{adModule} is not implemented!");
                    return false;
            }
        }


        /// <summary>
        /// Try show advertising by module type. Invoke callback with result of showing.
        /// </summary>
        /// <param name="module">The type of advertising module.</param>
        /// <param name="adShowingPlacement">The placement of showing ads.</param>
        /// <param name="callback">Invoke callback with result of showing.</param>
        public virtual void TryShowAdByModule(AdModule module, string adShowingPlacement,
            Action<AdActionResultType> callback = null)
        {
            // Banner will automatically show after loading them.
            if (module == AdModule.Banner)
            {
                ShowAdByModule(module, adShowingPlacement, callback);
            }
            
            if (ReachabilityHandler.Instance.NetworkStatus != NetworkStatus.NotReachable)
            {
                if (IsAdModuleByPlacementAvailable(module, adShowingPlacement))
                {
                    ShowAdByModule(module, adShowingPlacement, callback);
                }
                else
                {
                    SendAdAvailabilityCheckByModule(module, false, adShowingPlacement, AdAvailabilityErrorType.NoAdAvailable);
                    callback?.Invoke(AdActionResultType.NotAvailable);
                }
            }
            else
            {
                SendAdAvailabilityCheckByModule(module, false, adShowingPlacement, AdAvailabilityErrorType.NoInternet);
                callback?.Invoke(AdActionResultType.NoInternet);
            }
        }


        /// <summary>
        /// Try hide advertising by module type. Works only for banners.
        /// </summary>
        /// <param name="module">The type of advertising module.</param>
        /// <param name="callback">Invoke callback with result of showing.</param>
        public void TryHideAdByModule(AdModule module, Action<AdActionResultType> callback = null)
        {
            switch (module)
            {
                case AdModule.Banner:
                    isNeedShowBanner = false;
                    bannerController.HideBanner(callback);
                    break;
                case AdModule.Interstitial:
                    interstitialController.HideInterstitial(callback);
                    break;
                case AdModule.RewardedVideo:
                    rewardedVideoController.HideRewardedVideo(callback);
                    break;
                default:
                    CustomDebug.LogError($"{module} is not implemented!");
                    break;
            }
        }


        public void LockAd(AdModule adModule, string reason)
        {
            lockAdReasons[adModule].Add(reason);

            if (adModule == AdModule.Banner)
            {
                UpdateBannerVisibility(bannerController.AdvertisingPlacement);
            }
        }


        public void UnlockAd(AdModule adModule, string reason)
        {
            lockAdReasons[adModule].Remove(reason);
            
            if (adModule == AdModule.Banner)
            {
                UpdateBannerVisibility(bannerController.AdvertisingPlacement);
            }
        }
        
        
        protected bool IsInterstitialAvailable(string placementName)
        {
            bool result = !IsFullScreenAdShowing && lockAdReasons[AdModule.Interstitial].Count == 0;

            if (result)
            {
                result &= interstitialController.IsInterstitialAvailable(out _);
            }

            return result;
        }


        protected bool IsRewardedVideoAvailable(string placementName)
        {
            bool result = !IsFullScreenAdShowing && lockAdReasons[AdModule.RewardedVideo].Count == 0;

            if (result)
            {
                result &= rewardedVideoController.IsRewardedVideoAvailable(out _);
            }

            return result;
        }


        protected bool IsBannerAvailable(string placementName)
        {
            bool result = !IsFullScreenAdShowing && lockAdReasons[AdModule.Banner].Count == 0;

            if (result)
            {
                result &= bannerController.IsBannerAvailable(placementName, out _);
            }

            return result;
        }

        

        protected virtual bool IsCrossPromoInterstitialAvailable(string placementName)
        {
            bool result = !IsFullScreenAdShowing && lockAdReasons[AdModule.Interstitial].Count == 0;

            if (result)
            {
                result &= crossPromoController.IsInterstitialAvailable(placementName, out _);
            }

            return result;
        }


        protected void UpdateBannerVisibility(string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            bool isBannerVisibilityAvailable = isNeedShowBanner && 
                                               IsAdModuleByPlacementAvailable(AdModule.Banner, adShowingPlacement);

            if (isBannerVisibilityAvailable)
            {
                bannerController.ShowBanner(adShowingPlacement, callback);
            }
            else
            {
                bannerController.HideBanner();
            }
        }


        protected virtual void FillPreDefinedAvailabilityParameters()
        {
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, 
                AdAvailabilityParameterType.DefaultParameter, 
                IsInterstitialAvailable, int.MaxValue));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, 
                AdAvailabilityParameterType.DefaultParameter, 
                IsBannerAvailable, int.MaxValue));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.RewardedVideo, 
                AdAvailabilityParameterType.DefaultParameter, 
                IsRewardedVideoAvailable, int.MaxValue));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial,
                AdAvailabilityParameterType.DefaultCrossPromoAvailability,
                IsCrossPromoInterstitialAvailable, int.MaxValue));
        }


        protected virtual void FillPreDefinedPlacements()
        {
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.Interstitial, AdPlacementType.DefaultPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.DefaultParameter)
                }));

            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.RewardedVideo, AdPlacementType.DefaultPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.RewardedVideo,
                        AdAvailabilityParameterType.DefaultParameter)
                }));

            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.Banner, AdPlacementType.DefaultPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.DefaultParameter)
                }));
        }


        private void FillCustomAdAvailabilityParameters(List<AdAvailabilityParameter> adAvailabilityParameters)
        {
            if (adAvailabilityParameters != null)
            {
                foreach (AdAvailabilityParameter availabilityParameter in adAvailabilityParameters)
                {
                    availabilityParametersLib.Add(availabilityParameter);
                } 
            }
        }


        private void FillCustomPlacements(List<AdPlacementModel> advertisingPlacements)
        {
            if (advertisingPlacements != null)
            {
                AdPlacementBuilder adPlacementBuilder = 
                    new AdPlacementBuilder(availabilityParametersLib, adPlacementsController);
                
                foreach (AdPlacementModel advertisingPlacementModel in advertisingPlacements)
                {
                    adPlacementsController.AddPlacement(
                        adPlacementBuilder.CreatePlacementSpec(advertisingPlacementModel));
                } 
            }
        }


        private void ShowAdByModule(
            AdModule module,
            string adShowingPlacement,
            Action<AdActionResultType> callback = null)
        {
            OnAdBeforeShow?.Invoke(module);

            switch (module)
            {
                case AdModule.Interstitial:
                    bannerController.HideBanner();
                    interstitialController.ShowInterstitial(adShowingPlacement, callback);
                    break;
                case AdModule.RewardedVideo:
                    bannerController.HideBanner();
                    rewardedVideoController.ShowVideo(adShowingPlacement, callback);
                    break;
                case AdModule.Banner:
                    isNeedShowBanner = true;
                    bannerController.AdvertisingPlacement = adShowingPlacement;
                    UpdateBannerVisibility(adShowingPlacement, callback);
                    break;
                default:
                    CustomDebug.LogError($"{module} is not implemented!");
                    break;
            }
        }


        private void SendAdAvailabilityCheckByModule(
            AdModule module,
            bool isAdAvailable,
            string adShowingPlacement,
            string errorDescription)
        {
            switch (module)
            {
                case AdModule.Interstitial:
                    interstitialController.IsInterstitialAvailable(out var interstitialServiceInfo);
                    interstitialController.SendAdAvailabilityCheck(adShowingPlacement, isAdAvailable, interstitialServiceInfo, errorDescription);
                    break;
                case AdModule.RewardedVideo:
                    rewardedVideoController.IsRewardedVideoAvailable(out var rewardedVideoServiceInfo);
                    rewardedVideoController.SendAdAvailabilityCheck(adShowingPlacement, isAdAvailable, rewardedVideoServiceInfo, errorDescription);
                    break;
            }
        }

        #endregion
        
        
        
        #region ServicesInitializer
        
        protected override void AvailableServices_OnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            base.AvailableServices_OnCollectionChanged(sender, notifyCollectionChangedEventArgs);
            
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (notifyCollectionChangedEventArgs.NewItems[0] is IAdvertisingService service)
                    {
                        availableAdvertisingServices.Add(service.GetType(), service);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (notifyCollectionChangedEventArgs.OldItems[0] is IAdvertisingService service)
                    {
                        if (availableAdvertisingServices.TryGetValue(service.GetType(), out _))
                        {
                            availableAdvertisingServices.Remove(service.GetType());
                        }
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                    break;
            }
        }
        
        
        protected virtual void Controller_OnAdHide(
            AdModule adModule, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            switch (adModule)
            {
                case AdModule.Interstitial:
                case AdModule.RewardedVideo:
                    UpdateBannerVisibility(bannerController.AdvertisingPlacement);
                    break;
                default:
                    break;
            }
            
            OnAdHide?.Invoke(adModule, responseResultType);
        }


        protected virtual void Controller_OnAdRespond(
            AdModule adModule, 
            int delay, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            if (adModule == AdModule.Banner && responseResultType == AdActionResultType.Success)
            {
                UpdateBannerVisibility(bannerController.AdvertisingPlacement);
            }
            
            OnAdRespond?.Invoke(adModule, responseResultType);
        }
        
        
        protected virtual void Controller_OnAdShow(
            AdModule adModule, 
            AdActionResultType responseResultType, 
            int delay, 
            string errorDescription, 
            string adIdentifier, 
            string advertisingPlacement)
        {
            OnAdShow?.Invoke(adModule, responseResultType);
        }


        protected void Controller_OnAdClick(
            AdModule adModule, 
            string adIdentifier, 
            string advertisingPlacement)
        {
            OnAdClick?.Invoke(adModule);
        }
        
        #endregion
    }
}
