namespace Modules.Advertising
{
    public interface ICrossPromo
    {
        bool IsInterstitialAvailable { get; }
        
        bool IsSubscriptionPopupActive { get; set; }
        
        void ShowInterstitial();
    }
}
