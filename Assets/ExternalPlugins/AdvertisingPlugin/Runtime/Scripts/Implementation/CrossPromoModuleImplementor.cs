using Modules.General.Abstraction;


namespace Modules.Advertising
{
    public abstract class CrossPromoModuleImplementor : EventAdsImplementor, ICrossPromo
    {
        protected CrossPromoModuleImplementor(IAdvertisingService service) : base(service) { }
        
        public override AdModule AdModule => AdModule.Interstitial;
        
        public abstract bool IsInterstitialAvailable { get; protected set; }
        
        public abstract bool IsSubscriptionPopupActive { get; set; }
        
        public abstract void ShowInterstitial();
    }
}
