using Modules.General.HelperClasses;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Max
{
    [CreateAssetMenu(fileName = "LLMaxSettings")]
    public class LLMaxSettings : ScriptableSingleton<LLMaxSettings>
    {
        public readonly string[] ConsentApiClassesNames =
        {
            "LLAppsFlyerManager", "LLFirebaseManager", "LLTenjinManager", "LLTapjoyManager", "LLFacebookManager",
            "LLAdmobManager", "LLTapdaqManager", "LLAppLovinManager", "LLIronSourceManager", "LLAppodealManager",
            "AppLovinManager", "LLAppseeManager", "LLMintegralManager", "LLMoPubManager", "FirebaseManager",
            "AmplitudeManager"
        };
        
        public string SdkKey;
        
        [Space]
        public bool IsBannerEnabled = true;
        public string BannerId;

        [Space]
        public bool IsInterstitialEnabled = true;
        public string InterstitialId;

        [Space]
        public bool IsRewardedEnabled = true;
        public string RewardedId;

        [Space]
        public MaxSdkBase.BannerPosition BannerPosition = MaxSdkBase.BannerPosition.BottomCenter;

        public int PhoneBannerWidth = 320;

        public int TabletBannerWidth = 728;

        [Tooltip("Adaptive banners are sized based on device width for positions that stretch full width (TopCenter and BottomCenter)")]
        public bool EnableAdaptiveBanners = false;

        [Tooltip("Showing banners instead of leader on tablets")]
        public bool ForceBanners = false;
        
        public bool ShouldStopAutoRefreshOnAdExpand = true;
        
        [Space]
        [Header("Privacy")]
        public string PrivacyUrl = "https://aigames.ae/policy#h.hn0lb3lfd0ij";
        public string TermsUrl = "https://aigames.ae/policy#h.v7mztoso1wgw";

        
        
        
        [HideInInspector]
        public List<string> EnabledAdapters = new List<string>()
        {
            "ByteDanceAdapter",
            "FacebookAdapter",
            "FyberAdapter",
            "GoogleAdapter",
            "IronSourceAdapter",
            "MintegralAdapter",
            "UnityAdsAdapter",
            "VungleAdapter"
        };
        
        [HideInInspector] [SerializeField] private List<string> consentApiClassesNamesIncludingAssemblies = new List<string>();
        public List<string> ConsentApiClassesNamesIncludingAssemblies => consentApiClassesNamesIncludingAssemblies;
    }
}
