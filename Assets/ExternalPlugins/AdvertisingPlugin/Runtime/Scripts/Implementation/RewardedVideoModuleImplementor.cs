using Modules.General.Abstraction;


namespace Modules.Advertising
{
    public abstract class RewardedVideoModuleImplementor : EventAdsImplementor, IRewardedVideo
    {
        protected RewardedVideoModuleImplementor(IAdvertisingService service) : base(service) { }

        public override AdModule AdModule => AdModule.RewardedVideo;

        public abstract bool IsVideoAvailable { get; protected set; }
        public abstract void ShowVideo(string placementName);
    }
}
