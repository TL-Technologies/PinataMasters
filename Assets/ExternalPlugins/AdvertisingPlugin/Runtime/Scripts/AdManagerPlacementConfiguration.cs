using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public partial class AdvertisingManager
    {
        #region Properties

        private bool IsCrossPromoAvailable
        {
            get
            {
                AdPlacementSpec crossPromoPlacement = adPlacementsController.GetPlacement(AdModule.Interstitial,
                    AdPlacementType.DefaultCrossPromoPlacement);
                if (crossPromoPlacement != null)
                {
                    return crossPromoPlacement.IsAvailable;
                }
                
                return false;
            }
        }

        #endregion
        
        
        
        #region Methods

        protected override void FillPreDefinedAvailabilityParameters()
        {
            base.FillPreDefinedAvailabilityParameters();
            
            // Rewarded Video parameters
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.RewardedVideo, AdAvailabilityParameterType.PersonalDataNotDeleted,
                (placement => !AdvertisingNecessaryInfo.IsPersonalDataDeleted)));
            
            // Banner parameters
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, AdAvailabilityParameterType.NoActiveSubscription,
                (placement => !AdvertisingNecessaryInfo.IsSubscriptionActive)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, AdAvailabilityParameterType.NoShowingSubscription,
                (placement => !isSubscriptionShowing)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, AdAvailabilityParameterType.NoActiveNoAds,
                (placement => !AdvertisingNecessaryInfo.IsNoAdsActive)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, AdAvailabilityParameterType.PersonalDataNotDeleted,
                (placement => !AdvertisingNecessaryInfo.IsPersonalDataDeleted)));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Banner, AdAvailabilityParameterType.MinLevelForBannerShowing,
                (placement => AdvertisingNecessaryInfo.CurrentPlayerLevel > 
                              InGameAdvertisingAbTestData.minLevelForBannerShowing)));
            
            // CrossPromo parameters
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.CrossPromoAbTestDataExist,
                (placement => crossPromoAdvertisingAbTestData != null), int.MinValue + 1));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.UpdateCrossPromoDailyData,
                (placement =>
                {
                    crossPromoController.UpdateInterstitialDailyData();
                    
                    return true;
                }), int.MinValue));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.CrossPromoPresentationOffset,
                (placement =>
                {
                    int interstitialShowNumberWithXPromoOffset = interstitialTimesShown + 1 -
                                                                 CrossPromoAdvertisingAbTestData
                                                                     .xPromoFirstInterstitialShowNumber;
                    if (interstitialShowNumberWithXPromoOffset >= 0)
                    {
                        return interstitialShowNumberWithXPromoOffset %
                            CrossPromoAdvertisingAbTestData.xPromoIntersititalPresentationOffset == 0;
                    }

                    return false;
                })));
            
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.CrossPromoDailyLimit,
                (placement => crossPromoController.CountInterstitialShownPerDay <
                              CrossPromoAdvertisingAbTestData.xPromoInterstitialMaxPerDay)));
            
            // Interstitial parameters
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.NoActiveSubscription,
                (placement => !AdvertisingNecessaryInfo.IsSubscriptionActive)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.NoShowingSubscription,
                (placement => !isSubscriptionShowing)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.NoActiveNoAds,
                (placement => !AdvertisingNecessaryInfo.IsNoAdsActive)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.PersonalDataNotDeleted,
                (placement => !AdvertisingNecessaryInfo.IsPersonalDataDeleted)));

            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.DelayBetweenInterstitials,
                (placement => IsInterstitialDelayEnded)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.MinLevelForIntestitialShowing,
                (placement => AdvertisingNecessaryInfo.CurrentPlayerLevel >
                              InGameAdvertisingAbTestData.minLevelForInterstitialShowing)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.GalleryOpenInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowGalleryOpenInterstitials)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.GalleryCloseInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowGalleryCloseInterstitials)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.SettingsOpenInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowSettingsOpenInterstitials)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.SettingsCloseInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowSettingsCloseInterstitials)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.AfterBackgroundInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInterstitialAfterBackground)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.BeforeResultInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInterstitialBeforeResult)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.AfterResultInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInterstitialAfterResult)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.InGameRestartInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInGameRestartInterstitial)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.NineChestInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShow9ChestInterstitial)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.InactivityInterstitialAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInactivityInterstitial)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.DelayBetweenInactivityInterstitials,
                (placement => 
                    InactivityTime >= InGameAdvertisingAbTestData.delayBetweenInactivityInterstitials)));
            availabilityParametersLib.Add(new AdAvailabilityParameter(
                AdModule.Interstitial, AdAvailabilityParameterType.InterstitialBetweenLevelSegmentsAvailable,
                (placement => InGameAdvertisingAbTestData.isNeedShowInterstitialAfterSegment)));
        }


        protected override void FillPreDefinedPlacements()
        {
            base.FillPreDefinedPlacements();
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.Banner, AdPlacementType.DefaultPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.NoShowingSubscription),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.NoActiveSubscription),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.NoActiveNoAds),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.PersonalDataNotDeleted),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.MinLevelForBannerShowing),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Banner,
                        AdAvailabilityParameterType.DefaultParameter)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.RewardedVideo, AdPlacementType.DefaultPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.RewardedVideo,
                        AdAvailabilityParameterType.PersonalDataNotDeleted),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.RewardedVideo,
                        AdAvailabilityParameterType.DefaultParameter)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.Interstitial, AdPlacementType.DefaultCrossPromoPlacement,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial, 
                        AdAvailabilityParameterType.CrossPromoAbTestDataExist),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial, 
                        AdAvailabilityParameterType.UpdateCrossPromoDailyData),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial, 
                        AdAvailabilityParameterType.CrossPromoPresentationOffset),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial, 
                        AdAvailabilityParameterType.CrossPromoDailyLimit),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial, 
                        AdAvailabilityParameterType.DefaultCrossPromoAvailability)
                }));

            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdModule.Interstitial, AdPlacementType.DefaultPlacementWithoutTimeAndMinLevelLimit,
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.NoShowingSubscription),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.NoActiveSubscription),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.NoActiveNoAds),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.PersonalDataNotDeleted),
                },
                (placement => IsCrossPromoAvailable || IsInterstitialAvailable(placement))));

            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.DefaultPlacement, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacementWithoutTimeAndMinLevelLimit),
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.DelayBetweenInterstitials),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.MinLevelForIntestitialShowing)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.NineChests, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacementWithoutTimeAndMinLevelLimit),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.NineChestInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.GalleryOpen, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.GalleryOpenInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.GalleryClose, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.GalleryCloseInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.SettingsOpen, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.SettingsOpenInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.SettingsClose, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.SettingsCloseInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.Background, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                {
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.AfterBackgroundInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.Inactivity, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.InactivityInterstitialAvailable),
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                    AdAvailabilityParameterType.DelayBetweenInactivityInterstitials)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.InGameRestart, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.InGameRestartInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.BeforeResult, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.BeforeResultInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.AfterResult, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.AfterResultInterstitialAvailable)
                }));
            
            adPlacementsController.AddPlacement(new AdPlacementSpec(
                AdPlacementType.NextLevelSegment, 
                adPlacementsController.GetPlacement(AdModule.Interstitial, 
                    AdPlacementType.DefaultPlacement),
                new List<AdAvailabilityParameter>()
                { 
                    availabilityParametersLib.GetAdAvailabilityParameter(AdModule.Interstitial,
                        AdAvailabilityParameterType.InterstitialBetweenLevelSegmentsAvailable)
                }));
        }

        #endregion
    }
}