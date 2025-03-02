using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class DefaultAdvertisingTestData : IInGameAdvertisingAbTestData
    {
        public Dictionary<AdModule, float> advertisingAvailabilityEventInfo { get; set; } = 
            new Dictionary<AdModule, float>()
            {
                { AdModule.RewardedVideo, 10.0f },
                { AdModule.Interstitial, 15.0f },
                { AdModule.Banner, 5.0f },
            };

        public int minLevelForBannerShowing { get; set; } = 0;

        public int minLevelForInterstitialShowing  { get; set; } = 0;
        public float delayBetweenInterstitials  { get; set; } = 30.0f;

        public bool isNeedShowInterstitialBeforeResult { get; set; } = true;
        public bool isNeedShowInterstitialAfterResult  { get; set; } = false;

        public bool isNeedShowInterstitialAfterBackground  { get; set; } = true;

        public bool isNeedShowInactivityInterstitial { get; set; } = true;
        public float delayBetweenInactivityInterstitials { get; set; } = 60.0f;

        public bool isNeedShowSettingsOpenInterstitials { get; set; } = false;
        public bool isNeedShowSettingsCloseInterstitials { get; set; } = false;

        public bool isNeedShowGalleryOpenInterstitials { get; set; } = false;
        public bool isNeedShowGalleryCloseInterstitials { get; set; } = false;

        public bool isNeedShowInGameRestartInterstitial { get; set; } = false;

        public bool isNeedShow9ChestInterstitial { get; set; } = true;

        public bool isNeedShowInterstitialAfterSegment { get; set; } = true;
    }
}