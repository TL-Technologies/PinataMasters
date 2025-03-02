using Modules.General.Abstraction;


namespace Modules.Advertising
{
    public abstract class BannerModuleImplementor : EventAdsImplementor, IBanner
    {
        protected BannerModuleImplementor(IAdvertisingService service) : base(service) { }

        public override AdModule AdModule => AdModule.Banner;
        
        public abstract bool IsBannerAvailable { get; protected set; }
        public abstract void ShowBanner(string placementName);
        public abstract void HideBanner();
    }
}
