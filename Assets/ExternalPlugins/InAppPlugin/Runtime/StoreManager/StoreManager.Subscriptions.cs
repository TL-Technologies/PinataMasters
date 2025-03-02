using Modules.General.Abstraction.InAppPurchase;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    public sealed partial class StoreManager
    {
        private SubscriptionRewardManager subscriptionRewardManager;

        public ISubscriptionRewardManager SubscriptionRewardManager
        {
            get
            {
                return subscriptionRewardManager ?? (subscriptionRewardManager = new SubscriptionRewardManager(this));
            }
        }

        public bool IsFakeSubscriptionEnabled { get; set; }
        
        
        /// <summary>
        /// Gets whether a user has any active subscription.
        /// </summary>
        public bool HasAnyActiveSubscription =>
            IsFakeSubscriptionEnabled || GetActiveSubscriptions().Count > 0;
        
        
        /// <summary>
        /// Returns an instance of store item that describes an active subscription
        /// or null if there is no active subscription at this time.
        /// </summary>
        /// <returns></returns>
        public List<IStoreItem> GetActiveSubscriptions()
        {
            List<IStoreItem> result = new List<IStoreItem>();
            foreach (StoreItem item in storeItems.Where(item => item.IsSubscription))
            {
                bool isSubscriptionActive = item.SubscriptionInfo != null && item.SubscriptionInfo.IsSubscribed;
                if (isSubscriptionActive)
                {
                    result.Add(item);
                }
            }
            
            return result;
        }
        
        
        /// <summary>
        /// Gets whether the specified subscription allows a trial mode.
        /// </summary>
        /// <param name="identifier">The identifier of store item to check.</param>
        /// <returns></returns>
        public bool IsSubscriptionTrialAvailable(string identifier)
        {
            IStoreItem item = GetStoreItem(identifier);
            return IsSubscriptionTrialAvailable(item);
        }
        
        
        /// <summary>
        /// Gets whether the specified subscription allows a trial mode.
        /// </summary>
        /// <param name="item">The store item to check.</param>
        /// <returns></returns>
        public bool IsSubscriptionTrialAvailable(IStoreItem item)
        {
            if (item == null || !item.IsSubscription)
            {
                return false;
            }
            
            return purchaseHistoryHelper == null ||
                !purchaseHistoryHelper.WasItemPurchased(item) ||
                (item.SubscriptionInfo != null && item.SubscriptionInfo.IsFreeTrial);
        }
        
        
        // This method logic is got from IAPDemo.cs class in Unity purchase plugin
        private ISubscriptionInfo GetSubscriptionInfo(Product product)
        {
            #if UNITY_EDITOR
                // Editor have fake receipt for new purchase, but doesn't save receipts between restarts.
                // Try to restore it from PlayerPrefs
                if (FakeComputableSubscriptionInfo.HaveSubscriptionOfType(product.definition.storeSpecificId))
                {
                    SubscriptionInfo info = new SubscriptionManager(product, null).getSubscriptionInfo();
                    return new SubscriptionInfoImplementor(GetStoreItem(product.definition.id), info);
                }
            #endif
            
            if (product.definition.type == ProductType.Subscription && 
                product.hasReceipt &&
                IsProductAvailableForSubscriptionManager(product.receipt))
            {
                string introJson = null;
                if (appleIntroductoryPriceDictionary != null &&
                    appleIntroductoryPriceDictionary.ContainsKey(product.definition.storeSpecificId))
                {
                    introJson = appleIntroductoryPriceDictionary[product.definition.storeSpecificId];
                }
                CustomDebug.Log($"[StoreManager - GetSubscriptionInfo] {product.definition.id}, receipt : {product.receipt}");
                SubscriptionInfo info = new SubscriptionManager(product, introJson).getSubscriptionInfo();

                return new SubscriptionInfoImplementor(GetStoreItem(product.definition.id), info);
            }
            return null;
            
            
            bool IsProductAvailableForSubscriptionManager(string receipt)
            {
                Dictionary<string, object> receiptWrapper = JsonConvert.DeserializeObject<Dictionary<string, object>>(receipt);
                if (!receiptWrapper.ContainsKey("Store") || !receiptWrapper.ContainsKey("Payload"))
                {
                    CustomDebug.Log("StoreManager: the product receipt does not contain enough information");
                    return false;
                }
                
                string payload = (string)receiptWrapper["Payload"];
                if (payload != null)
                {
                    string store = (string)receiptWrapper["Store"];
                    switch (store)
                    {
                        case GooglePlay.Name:
                        case AppleAppStore.Name:
                        case AmazonApps.Name:
                        case MacAppStore.Name:
                        case HuaweiAppGallery.Name:
                        #if UNITY_EDITOR
                            case "fake":
                        #endif
                            return true;
                        default:
                            return false;
                    }
                }
                
                return false;
            }
        }
    }
}
