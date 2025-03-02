namespace Modules.Advertising
{
    public interface IBanner : IEventAds
    {
        bool IsBannerAvailable { get; }
        
        void ShowBanner(string placementName);
        void HideBanner();
    }
}
