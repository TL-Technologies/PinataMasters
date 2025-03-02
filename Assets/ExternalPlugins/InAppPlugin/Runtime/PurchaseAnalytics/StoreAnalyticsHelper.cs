using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.General.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.InAppPurchase
{
    internal class StoreAnalyticsHelper
    {
        #region Fields
        
        private IAdvertisingIdentifier advertisingIdentifierService;
        private IStoreSettings storeSettingsService;
        private Dictionary<string, string> developerPayload;
        private IPurchaseAnalytics purchaseAnalyticsService;
        private IPurchaseAnalyticsParameters purchaseAnalyticsParametersService;
        
        #endregion



        #region Properties

        public string DeveloperPayload
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>(developerPayload);
                foreach (KeyValuePair<string, Func<string>> pair in purchaseAnalyticsParametersService.Parameters)
                {
                    result.Add(pair.Key, pair.Value?.Invoke());
                }
                
                return JsonConvert.SerializeObject(result);
            }
        }
        
        #endregion



        #region Class lifecycle

        internal StoreAnalyticsHelper(
            IStoreSettings storeSettings,
            IAdvertisingIdentifier advertisingIdentifier,
            IPurchaseAnalytics purchaseAnalytics,
            IPurchaseAnalyticsParameters purchaseAnalyticsParameters)
        {
            storeSettingsService = storeSettings;
            advertisingIdentifierService = advertisingIdentifier;
            
            purchaseAnalyticsService = purchaseAnalytics;
            if (purchaseAnalyticsService == null)
            {
                purchaseAnalyticsService = new PurchaseAnalyticsDummy();
                CustomDebug.Log("StoreManager will use dummy purchase analytics!");
            }
            purchaseAnalyticsParametersService = purchaseAnalyticsParameters;
            if (purchaseAnalyticsParametersService == null)
            {
                purchaseAnalyticsParametersService = new PurchaseAnalyticsParametersDummy();
                CustomDebug.Log("StoreManager will use dummy purchase analytics parameters!");
            }
            
            advertisingIdentifierService.GetAdvertisingIdentifier(id =>
            {
                developerPayload = new Dictionary<string, string>
                {
                    { "appsflyer_id", purchaseAnalyticsService.AnalyticsUserId },
                    { "advertising_id", id },
                    { "os_version", Application.version },
                    { "app_version", DeviceInfo.OperationSystemVersion }
                };
            });
        }
        
        #endregion



        #region Methods
        
        public async Task SendPurchaseCompleteEventAsync(PurchaseItemResult result)
        {
            // Accept only successfully purchased transactions
            if (result.TransactionState != PurchaseTransactionState.Purchased)
            {
                return;
            }

            if (result.StoreItem.IsSubscription)
            {
                return;
            }

            // For all other shop items => send event to purchase analytics (aka AppsFlyer)
            purchaseAnalyticsService.LogPurchase(
                result.StoreItem.ProductId,
                result.StoreItem.CurrencyCode,
                result.StoreItem.RealPrice,
                result.TransactionId,
                result.ReceiptParser.GooglePlayJson,
                result.ReceiptParser.GooglePlaySignature,
                storeSettingsService.GooglePlayAppPublicKey);
        }

        #endregion
    }
}
