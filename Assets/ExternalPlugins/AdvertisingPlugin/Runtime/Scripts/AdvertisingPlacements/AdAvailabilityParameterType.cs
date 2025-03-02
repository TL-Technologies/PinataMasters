namespace Modules.Advertising
{
    public class AdAvailabilityParameterType
    {
        #region General parameters

        public const string DefaultParameter = "Default";

        public const string NoActiveSubscription = "NoActiveSubscription";

        public const string NoShowingSubscription = "NoShowingSubscription";
        
        public const string NoActiveNoAds = "NoActiveNoAds";

        public const string PersonalDataNotDeleted = "PersonalDataNotDeleted";

        #endregion
        
        
        #region Interstitial parameters

        public const string DelayBetweenInterstitials = "DelayBetweenInterstitials";

        public const string MinLevelForIntestitialShowing = "MinLevelForIntestitialShowing";

        public const string GalleryOpenInterstitialAvailable = "GalleryOpenInterstitialAvailable";

        public const string GalleryCloseInterstitialAvailable = "GalleryCloseInterstitialAvailable";

        public const string SettingsOpenInterstitialAvailable = "SettingsOpenInterstitialAvailable";

        public const string SettingsCloseInterstitialAvailable = "SettingsCloseInterstitialAvailable";

        public const string AfterBackgroundInterstitialAvailable = "AfterBackgroundInterstitialAvailable";

        public const string BeforeResultInterstitialAvailable = "BeforeResultInterstitialAvailable";

        public const string AfterResultInterstitialAvailable = "AfterResultInterstitialAvailable";

        public const string InGameRestartInterstitialAvailable = "InGameRestartInterstitialAvailable";
        
        public const string NineChestInterstitialAvailable = "NineChestInterstitialAvailable";

        public const string InactivityInterstitialAvailable = "InactivityInterstitialAvailable";
        
        public const string InterstitialBetweenLevelSegmentsAvailable = "InterstitialBetweenLevelSegmentsAvailable";

        public const string DelayBetweenInactivityInterstitials = "DelayBetweenInactivityInterstitials";
        
        #endregion



        #region Crosspromo parameters

        public const string CrossPromoAbTestDataExist = "CrossPromoAbTestDataExist";

        public const string UpdateCrossPromoDailyData = "UpdateCrossPromoDailyData";

        public const string CrossPromoPresentationOffset = "CrossPromoPresentationOffset";

        public const string CrossPromoDailyLimit = "CrossPromoDailyLimit";

        public const string DefaultCrossPromoAvailability = "DefaultCrossPromoAvailability";

        #endregion

        
        
        #region Banner parameters

        public const string MinLevelForBannerShowing = "MinLevelForBannerShowing";

        #endregion
    }
}
