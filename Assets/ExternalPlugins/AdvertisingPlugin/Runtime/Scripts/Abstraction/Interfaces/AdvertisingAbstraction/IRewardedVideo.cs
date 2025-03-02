namespace Modules.Advertising
{
    public interface IRewardedVideo : IEventAds
    {
        bool IsVideoAvailable { get; }
        
        void ShowVideo(string placementName);
    }
}
