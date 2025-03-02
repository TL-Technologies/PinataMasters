using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    public abstract class EventAdsImplementor : IEventAds
    {
        #region Fields

        public event Action<IAdvertisingService, AdModule, string, string, float> OnImpressionDataReceive;
        public event Action<IAdvertisingService, AdModule, string> OnAdRequested;
        public event Action<IAdvertisingService, AdModule, int, AdActionResultType, string, string> OnAdRespond;
        public event Action<IAdvertisingService, AdModule, AdActionResultType, int, string, string> OnAdShow;
        public event Action<IAdvertisingService, AdModule, AdActionResultType, string, string> OnAdHide;
        public event Action<IAdvertisingService, AdModule, string> OnAdClick;
        public event Action<IAdvertisingService, AdModule, int, string> OnAdExpire;

        private IAdvertisingService advertisingService;

        #endregion



        #region Properties

        public abstract AdModule AdModule { get; }

        #endregion



        #region Class lifecycle

        public EventAdsImplementor(IAdvertisingService service)
        {
            advertisingService = service;
        }

        #endregion



        #region Methods

        public void Invoke_OnImpressionDataReceive(string impressionJsonData, string adIdentifier,
            float revenue)
        {
            OnImpressionDataReceive?.Invoke(advertisingService, AdModule, impressionJsonData, adIdentifier, revenue);
        }


        public void Invoke_OnAdRequested(string adIdentifier)
        {
            OnAdRequested?.Invoke(advertisingService, AdModule, adIdentifier);
        }


        public virtual void Invoke_OnAdRespond(int delay, AdActionResultType responseResultType,
            string errorDescription, string adIdentifier)
        {
            OnAdRespond?.Invoke(advertisingService, AdModule, delay, responseResultType, errorDescription,
                adIdentifier);
        }


        public virtual void Invoke_OnAdShow(AdActionResultType responseResultType, int delay,
            string errorDescription, string adIdentifier)
        {
            OnAdShow?.Invoke(advertisingService, AdModule, responseResultType, delay, errorDescription, adIdentifier);
        }


        public virtual void Invoke_OnAdHide(AdActionResultType responseResultType, string errorDescription,
            string adIdentifier)
        {
            OnAdHide?.Invoke(advertisingService, AdModule, responseResultType, errorDescription, adIdentifier);
        }


        public void Invoke_OnAdClick(string adIdentifier)
        {
            OnAdClick?.Invoke(advertisingService, AdModule, adIdentifier);
        }


        public void Invoke_OnAdExpire(int delay, string adIdentifier)
        {
            OnAdExpire?.Invoke(advertisingService, AdModule, delay, adIdentifier);
        }

        #endregion
    }
}
