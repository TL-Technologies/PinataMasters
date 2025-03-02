using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    public interface IEventAds
    {
        #region Fields

        event Action<IAdvertisingService, AdModule, string, string, float> OnImpressionDataReceive;
        event Action<IAdvertisingService, AdModule, string> OnAdRequested;
        event Action<IAdvertisingService, AdModule, int, AdActionResultType, string, string> OnAdRespond;

        event Action<IAdvertisingService, AdModule, AdActionResultType, int, string, string> OnAdShow;
        event Action<IAdvertisingService, AdModule, AdActionResultType, string, string> OnAdHide;
        event Action<IAdvertisingService, AdModule, string> OnAdClick;
        event Action<IAdvertisingService, AdModule, int, string> OnAdExpire;

        #endregion



        #region Methods

        void Invoke_OnImpressionDataReceive(
            string impressionJsonData, 
            string adIdentifier, 
            float revenue);
        
        
        void Invoke_OnAdRequested(
            string adIdentifier);
        
        
        void Invoke_OnAdRespond(
            int delay, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier);
        
        
        void Invoke_OnAdShow(
            AdActionResultType responseResultType, 
            int delay, 
            string errorDescription, 
            string adIdentifier);
        
        
        void Invoke_OnAdHide(
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier);
        
        
        void Invoke_OnAdClick(
            string adIdentifier);
        
        
        void Invoke_OnAdExpire(
            int delay, 
            string adIdentifier);

        #endregion
    }
}
