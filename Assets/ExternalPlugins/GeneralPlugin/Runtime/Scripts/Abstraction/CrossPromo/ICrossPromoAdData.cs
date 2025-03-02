namespace Modules.General.Abstraction
{
    public interface ICrossPromoAdData
    {
        string AppId { get; }
        string AppName { get; }
        string StoreId { get; }
        string StoreUrl { get; }
        string DeepLink { get; }
        string VideoPortraitUrl { get; }
        string VideoLandscapeUrl { get; }
        string CampaignName { get; }
        string CampainId { get; }
        string ClickLookbackPeriod { get; }
        string ViewthroughLookbackPeriod { get; }
        string MediaSource { get; }
        string PortraitContentName { get; }
        string PromotionId { get; }
        string PromotionName { get; }
        string LandscapeContentName { get; }
        string CurrentAppId { get; }
    }
}
