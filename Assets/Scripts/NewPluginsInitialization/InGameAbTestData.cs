using Modules.Advertising;
using Modules.General.Abstraction;
using System.Collections.Generic;


public class InGameAbTestData : IInGameAdvertisingAbTestData
{
    //remotes from firebase and abtests

    public string autotap = "true";
    public string difficulty = "normal";
    public bool isSubscriptionShowEnterBackground = false;
    public bool isVideoShopItemEnabled = true;
    public bool isCoinsCheckForVideoShopItemEnabled = false;
    public float videoShopItemRewardMultiplier = 2.0f;
    public float videoShopItemShowingDelay = 30.0f;
    public bool skipStory = true;
    public float offlineRewardMultiplier = 10.0f;
    public int x2VideoFrequencyLoss = 1;
    public int x2VideoFrequencyWin = 4;
    public float boosterCooldownPositive = 50.0f;
    public float boosterCooldownNegative = 90.0f;
    public bool boosterUseRealTime = false;
    public int boosterUnlockLevel = 3;
    public float boosterSpawnDelay = 2.0f;
    public float boosterLifetime = 15.0f;
    
    public Dictionary<AdModule, float> advertisingAvailabilityEventInfo { get; set; } = 
        new Dictionary<AdModule, float>()
        {
            { AdModule.RewardedVideo, 10.0f },
            { AdModule.Interstitial, 15.0f },
            { AdModule.Banner, 5.0f },
        };

    public int minLevelForBannerShowing { get; set; } = 5;

    public int minLevelForInterstitialShowing  { get; set; } = 0;
    public float delayBetweenInterstitials  { get; set; } = 30.0f;

    public bool isNeedShowInterstitialBeforeResult { get; set; } = false;
    public bool isNeedShowInterstitialAfterResult  { get; set; } = true;

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
