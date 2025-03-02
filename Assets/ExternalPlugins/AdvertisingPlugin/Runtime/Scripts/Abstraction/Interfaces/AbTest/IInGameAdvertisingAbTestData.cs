using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public interface IInGameAdvertisingAbTestData : IAdvertisingAbTestData
    {
        Dictionary<AdModule, float> advertisingAvailabilityEventInfo { get; set; }

        int minLevelForBannerShowing { get; set; }

        int minLevelForInterstitialShowing { get; set; }
        float delayBetweenInterstitials { get; set; }

        bool isNeedShowInterstitialBeforeResult { get; set; }
        bool isNeedShowInterstitialAfterResult { get; set; }

        bool isNeedShowInterstitialAfterBackground { get; set; }

        bool isNeedShowInactivityInterstitial { get; set; }
        float delayBetweenInactivityInterstitials { get; set; }

        bool isNeedShowSettingsOpenInterstitials { get; set; }
        bool isNeedShowSettingsCloseInterstitials { get; set; }

        bool isNeedShowGalleryOpenInterstitials { get; set; }
        bool isNeedShowGalleryCloseInterstitials { get; set; }

        bool isNeedShowInGameRestartInterstitial { get; set; }

        bool isNeedShow9ChestInterstitial { get; set; }

        bool isNeedShowInterstitialAfterSegment { get; set; }
    }
}