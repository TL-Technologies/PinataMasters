namespace Modules.General.Abstraction
{
    public interface ICrossPromoAdvertisingAbTestData : IAdvertisingAbTestData
    {
        int xPromoIntersititalPresentationOffset { get; set; }
        int xPromoInterstitialMaxPerDay { get; set; }
        int xPromoFirstInterstitialShowNumber { get; set; }
    }
}
