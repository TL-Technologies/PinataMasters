using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.AppsFlyer
{
    public class AppsFlyerAnalyticsServiceImplementor : IAnalyticsService
    {
        #region Fields
        
        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        private string deviceId;
        private AppsFlyerSettings appsFlyerSettings;

        #endregion



        #region Properties
        
        public bool IsAsyncInitializationEnabled =>
            #if UNITY_IOS
                true;  
            #elif UNITY_ANDROID
                false;
            #else
                true;
            #endif

        public bool IsAsyncWorkAvailable => true;

        #endregion



        #region Class lifecycle

        public AppsFlyerAnalyticsServiceImplementor(AppsFlyerSettings settings)
        {
            appsFlyerSettings = settings;
        }

        #endregion



        #region Methods

        public void PreInitialize() { }


        public void SetDeviceId(string deviceId)
        {
            this.deviceId = deviceId;
        }


        public void SetUserConsent(bool isConsentAvailable)
        {
            LLAppsFlyerManager.SetUserConsent(isConsentAvailable);
        }


        public void Initialize()
        {
            LLAppsFlyerManager.OnAppsFlyerInit += LLAppsFlyerManager_OnAppsFlyerInit;
            LLAppsFlyerManager.Initialize(appsFlyerSettings, deviceId);
        }


        public void SendEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (IsAsyncWorkAvailable)
            {
                Task.Run(() =>
                {
                    LLAppsFlyerManager.LogRichEvent(eventName, parameters);
                });
            }
            else
            {
                LLAppsFlyerManager.LogRichEvent(eventName, parameters);
            }
        }


        public void SetUserProperty(string name, string value) {}


        public static void LogPurchase(
            string productName,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson = "",
            string androidPurchaseSignature = "",
            string androidPublicKey = "")
        {
            LLAppsFlyerManager.LogPurchase(
                productName, 
                currencyCode, 
                price, 
                transactionId, 
                androidPurchaseDataJson, 
                androidPurchaseSignature, 
                androidPublicKey);
        }

        
        public string LLAppsFlyerGetAppsFlyerUID()
        {
            return LLAppsFlyerManager.LLAppsFlyerGetAppsFlyerUID();
        }


        public void TrackCrossPromoImpression(
            string appId,
            IReadOnlyDictionary<string, string> parameters,
            Action<bool> callback = null)
        {
            LLAppsFlyerManager.TrackCrossPromoImpression(appId, parameters, callback);
        }


        public void TrackCrossPromoClick(
            string appId,
            IReadOnlyDictionary<string, string> parameters)
        {
            LLAppsFlyerManager.TrackCrossPromoClick(appId, parameters);
        }

        #endregion



        #region Events handlers

        private void LLAppsFlyerManager_OnAppsFlyerInit(
            LLAppsFlyerManager.InitializationStatus appsFlyerInitializationStatus)
        {
            LLAppsFlyerManager.OnAppsFlyerInit -= LLAppsFlyerManager_OnAppsFlyerInit;

            InitializationStatus analyticsInitializationStatus = InitializationStatus.None;
            switch (appsFlyerInitializationStatus)
            {
                case LLAppsFlyerManager.InitializationStatus.Success:
                    analyticsInitializationStatus = InitializationStatus.Success;
                    break;
                case LLAppsFlyerManager.InitializationStatus.None:
                case LLAppsFlyerManager.InitializationStatus.Failed:
                    analyticsInitializationStatus = InitializationStatus.Failed;
                    break;
                case LLAppsFlyerManager.InitializationStatus.Warning:
                    analyticsInitializationStatus = InitializationStatus.Warning;
                    break;
            }
            
            OnServiceInitialized?.Invoke(this, analyticsInitializationStatus);
        }

        #endregion
    }
}
