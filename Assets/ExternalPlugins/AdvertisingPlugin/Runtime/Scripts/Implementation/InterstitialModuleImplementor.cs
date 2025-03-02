using Modules.General.Abstraction;


namespace Modules.Advertising
{
    public abstract class InterstitialModuleImplementor : EventAdsImplementor, IInterstitial
    {
        protected InterstitialModuleImplementor(IAdvertisingService service) : base(service) { }

        public override AdModule AdModule => AdModule.Interstitial;

        public abstract bool IsInterstitialAvailable { get; protected set; }
        public abstract void ShowInterstitial(string placementName);
    }
}
