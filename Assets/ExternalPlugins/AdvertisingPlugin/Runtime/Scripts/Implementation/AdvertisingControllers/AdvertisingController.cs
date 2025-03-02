using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Modules.Advertising
{
    internal class AdvertisingController<T> where T : EventAdsImplementor
    {
        #region Fields
        
        protected readonly List<AdvertisingServiceInfo<T>> advertisingServiceInfos = 
            new List<AdvertisingServiceInfo<T>>();
        
        public event Action<AdModule, int, AdActionResultType, string, string> OnAdRespond;

        public event Action<AdModule, AdActionResultType, int, string, string, string> OnAdShow;
        public event Action<AdModule, AdActionResultType, string, string> OnAdHide;
        public event Action<AdModule, string, string> OnAdClick;

        internal AdAnalyticsTracker analyticsTracker;
        
        protected string advertisingPlacement = "EMPTY_PLACEMENT";
        protected Action<AdActionResultType> advertisingHideCallback;

        #endregion



        #region Properties

        public bool IsShowing { get; protected set; }

        
        public string AdvertisingPlacement
        {
            get => advertisingPlacement;
            set => advertisingPlacement = value;
        }

        #endregion



        #region Methods

        public void Initialize(IAdvertisingService[] advertisingServices, AdAnalyticsTracker adAnalyticsTracker)
        {
            analyticsTracker = adAnalyticsTracker;
            
            foreach (IAdvertisingService advertisingService in advertisingServices)
            {
                List<IEventAds> adsImplementors =
                    advertisingService.SupportedAdsModules.Where((implementor => implementor is T)).ToList();

                if (adsImplementors.Count > 0)
                {
                    IEventAds implementor = adsImplementors.First();
                    advertisingService.OnServiceInitialized += Service_OnServiceInitialized;
                    SubscribeOnServiceEvents(implementor as T);
                }
            }
        }


        protected virtual void SubscribeOnServiceEvents(T service)
        {
            service.OnAdClick += AdvertisingService_OnAdClick;
            service.OnAdShow += AdvertisingService_OnAdShow;
            service.OnAdHide += AdvertisingService_OnAdHide;
            service.OnAdExpire += AdvertisingService_OnAdExpire;
            service.OnAdRequested += AdvertisingService_OnAdRequested;
            service.OnAdRespond += AdvertisingService_OnAdRespond;
            service.OnImpressionDataReceive += AdvertisingService_OnImpressionDataReceive;
        }

        #endregion



        #region Events handlers

        private void Service_OnServiceInitialized(
            IInitializable initializable, 
            InitializationStatus initializationStatus)
        {
            if (initializationStatus == InitializationStatus.Success)
            {
                initializable.OnServiceInitialized -= Service_OnServiceInitialized;
                IAdvertisingService advertisingService = initializable as IAdvertisingService;
                if (advertisingService != null)
                {
                    IEventAds eventAdsImplementor = 
                        advertisingService.SupportedAdsModules.Where((implementor => implementor is T)).First();
                    if (eventAdsImplementor != null)
                    {
                        advertisingServiceInfos.Add(new AdvertisingServiceInfo<T>(
                            advertisingService, eventAdsImplementor as T, string.Empty));
                    }
                }
            }
        }
        
        
         private void AdvertisingService_OnImpressionDataReceive(
            IAdvertisingService service, 
            AdModule adModule, 
            string impressionJsonData, 
            string adIdentifier, 
            float revenue)
        {
            analyticsTracker.SendImpressionDataReceive(
                    service, 
                    adModule, 
                    impressionJsonData, 
                    adIdentifier, 
                    revenue);
            
            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} ImpressionDataReceive: " +
                            $"AdType = {adModule}; AdId = {adIdentifier};");
            #endif
        }

        
        private void AdvertisingService_OnAdRespond(
            IAdvertisingService service, 
            AdModule adModule, 
            int delay, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                AdvertisingServiceInfo<T> advertisingServiceInfo = advertisingServiceInfos.Find(
                    (info => info.advertisingService == service));

                if (advertisingServiceInfo != null)
                {
                    advertisingServiceInfo.unitId = adIdentifier;
                }
            }

            if (!string.Equals(service.ServiceName, "crosspromo"))
            {
                analyticsTracker.SendAdRespond(
                    service,
                    adModule,
                    delay,
                    responseResultType,
                    errorDescription,
                    adIdentifier);
            }

            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdRespond: AdType = {adModule}; " +
                            $"AdId = {adIdentifier}; Result = {responseResultType}; " +
                            $"Error: {errorDescription}; Delay = {delay}");
            #endif
            
            OnAdRespond?.Invoke(
                adModule, 
                delay, 
                responseResultType, 
                errorDescription, 
                adIdentifier);
        }

        
        private void AdvertisingService_OnAdRequested(
            IAdvertisingService service, 
            AdModule adModule, 
            string adIdentifier)
        {
            analyticsTracker.SendAdRequested(
                service, 
                adModule,
                adIdentifier);
            
            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdRequested: " +
                            $"AdType = {adModule}; AdId = {adIdentifier}");
            #endif
        }

        
        private void AdvertisingService_OnAdExpire(
            IAdvertisingService service, 
            AdModule adModule, 
            int delay, 
            string adIdentifier)
        {
            analyticsTracker.SendAdExpire(
                service, 
                adModule,
                delay,
                adIdentifier);
            
            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdExpire: " +
                            $"AdType = {adModule}; AdId = {adIdentifier}; Delay = {delay}");
            #endif
        }

        
        private void AdvertisingService_OnAdHide(
            IAdvertisingService service, 
            AdModule adModule, 
            AdActionResultType responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            IsShowing = false;
            
            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdHide: AdType = {adModule}; " +
                            $"AdId = {adIdentifier}; Result = {responseResultType}; Error: {errorDescription};");
            #endif

            OnAdHide?.Invoke(
                adModule, 
                responseResultType, 
                errorDescription, 
                adIdentifier);

            advertisingHideCallback?.Invoke(responseResultType);
        }

        
        protected virtual void AdvertisingService_OnAdShow(
            IAdvertisingService service, 
            AdModule adModule, 
            AdActionResultType responseResultType, 
            int delay, 
            string errorDescription, 
            string adIdentifier)
        {
            if (!string.Equals(service.ServiceName, "crosspromo"))
            {
                analyticsTracker.SendAdShow(
                    service,
                    adModule,
                    responseResultType,
                    delay,
                    errorDescription,
                    adIdentifier,
                    advertisingPlacement);
            }

            if (responseResultType == AdActionResultType.Error)
            {
                IsShowing = false;
                advertisingHideCallback?.Invoke(AdActionResultType.Error);
            }

            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdShow: AdType = {adModule}; " +
                            $"AdId = {adIdentifier}; Result = {responseResultType}; " +
                            $"Error: {errorDescription}; Delay = {delay}");
            #endif
            
            OnAdShow?.Invoke(
                adModule, 
                responseResultType, 
                delay, 
                errorDescription, 
                adIdentifier, 
                advertisingPlacement);
        }


        private void AdvertisingService_OnAdClick(
            IAdvertisingService service,
            AdModule adModule,
            string adIdentifier)
        {
            if (!string.Equals(service.ServiceName, "crosspromo"))
            {
                analyticsTracker.SendAdClick(
                    service,
                    adModule,
                    adIdentifier,
                    advertisingPlacement);
            }

            #if !UNITY_EDITOR
                CustomDebug.Log($"On {service.ServiceName} AdClick: AdType = {adModule}; " +
                                $"AdId = {adIdentifier};");
            #endif
            
            OnAdClick?.Invoke(adModule, adIdentifier, advertisingPlacement);
        }

        #endregion
    }
}
