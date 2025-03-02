using Modules.Analytics;
using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    internal class AdAnalyticsTracker
    {
        #region Fields

        private Dictionary<AdModule, string> moduleToType =
            new Dictionary<AdModule, string>
            {
                { AdModule.None, "None"},
                { AdModule.Banner, "banner" },
                { AdModule.Interstitial, "interstitial" },
                { AdModule.RewardedVideo, "video" }
            };

        #endregion
        
        
        
        #region Methods
        
        public void SendAdRequested(
            IAdvertisingService service, 
            AdModule adModule, 
            string adIdentifier)
        {
            if (adModule == AdModule.Banner)
            {
                return;
            }

            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdRequest(
                service.ServiceName, 
                moduleName, 
                adIdentifier);
        }


        public void SendAdRespond(
            IAdvertisingService service, 
            AdModule adModule, 
            int delay,
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            if (adModule == AdModule.Banner)
            {
                return;
            }
            
            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdRespond(
                service.ServiceName, 
                moduleName, delay, 
                responseResultType.ToString(), 
                errorDescription, 
                adIdentifier);
        }


        public void SendImpressionDataReceive(
            IAdvertisingService service, 
            AdModule adModule, 
            string impressionJsonData, 
            string adIdentifier, 
            float revenue)
        {
            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendImpressionData(
                service.ServiceName, 
                impressionJsonData, 
                adIdentifier, 
                revenue);
        }
        

        public void SendAdShow(
            IAdvertisingService service, 
            AdModule adModule, 
            AdActionResultType responseResultType,
            int delay, 
            string errorDescription, 
            string adIdentifier, 
            string placement)
        {
            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdShow(
                service.ServiceName, 
                moduleName, 
                responseResultType.ToString(), 
                delay, 
                errorDescription, 
                adIdentifier, 
                placement);
        }
        
        
        public void SendAdClick(
            IAdvertisingService service, 
            AdModule adModule, 
            string adIdentifier, 
            string placement)
        {
            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdClick(
                service.ServiceName,
                moduleName, 
                adIdentifier, 
                placement);
        }


        public void SendAdExpire(
            IAdvertisingService service, 
            AdModule adModule, 
            int delay, 
            string adIdentifier)
        {
            if (adModule == AdModule.Banner)
            {
                return;
            }
            
            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdExpire(
                service.ServiceName,
                moduleName, 
                delay, 
                adIdentifier);
        }


        public void SendAdAvailabilityCheck(
            IAdvertisingService service, 
            AdModule adModule, 
            bool adAvailabilityStatus, 
            string placementName,
            string errorDescription)
        {
            if (adModule == AdModule.Banner)
            {
                return;
            }

            moduleToType.TryGetValue(adModule, out string moduleName);
            CommonEvents.SendAdAvailabilityCheck(
                service.ServiceName,
                moduleName,
                adAvailabilityStatus,
                placementName,
                errorDescription);
        }

        #endregion
    }
}