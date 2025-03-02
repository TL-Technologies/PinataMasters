namespace Modules.Advertising
{
    public interface IInterstitial : IEventAds
    {
        bool IsInterstitialAvailable { get; }
        
        void ShowInterstitial(string placementName);
    }
}
