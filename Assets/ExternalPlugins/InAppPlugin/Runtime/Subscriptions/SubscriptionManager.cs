using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
// ReSharper disable BadSwitchBracesIndent
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable EnforceIfStatementBraces
// ReSharper disable InconsistentNaming
// ReSharper disable MultipleSpaces
// ReSharper disable TabsAndSpacesMismatch
// ReSharper disable WrongIndentSize


namespace Modules.InAppPurchase
{
    // This class duplicates class UnityEngine.Purchasing.SubscriptionManager, but:
    // 1. Replaces using UnityEngine.Purchasing.SubscriptionInfo by Modules.InAppPurchase.SubscriptionInfo
    // due to reasons which are described in the CustomSubscriptionInfo.cs file.
    // 2. Adds payload as a parameter for Amazon subscription info.
    // Code formatting remains similar to original code for the next updates alleviating.
    internal class SubscriptionManager
    {
        private string receipt;
        private string productId;
        private string intro_json;

        public static void UpdateSubscription(Product newProduct, Product oldProduct, string developerPayload, Action<Product, string> appleStore, Action<string, string> googleStore) {
            if (oldProduct.receipt == null) {
                Debug.Log("The product has not been purchased, a subscription can only be upgrade/downgrade when has already been purchased");
                return;
            }
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(oldProduct.receipt);
            if (receipt_wrapper == null || !receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload")) {
                Debug.Log("The product receipt does not contain enough information");
                return;
            }
            var store = (string)receipt_wrapper ["Store"];
            var payload = (string)receipt_wrapper ["Payload"];

            if (payload != null ) {
                switch (store) {
                case "GooglePlay":
                    {
                        SubscriptionManager oldSubscriptionManager = new SubscriptionManager(oldProduct, null);
                        SubscriptionInfo oldSubscriptionInfo = null;
                        try {
                            oldSubscriptionInfo = oldSubscriptionManager.getSubscriptionInfo();
                        } catch (Exception e) {
                            Debug.unityLogger.Log("Error: the product that will be updated does not have a valid receipt", e);
                            return;
                        }
                        string newSubscriptionId = newProduct.definition.storeSpecificId;
                        googleStore(oldSubscriptionInfo.getSubscriptionInfoJsonString(), newSubscriptionId);
                        return;
                    }
                case "AppleAppStore":
                case "MacAppStore":
                    {
                        appleStore(newProduct, developerPayload);
                        return;
                    }
                default:
                    {
                        Debug.Log("This store does not support update subscriptions");
                        return;
                    }
                }
            }
        }

        public static void UpdateSubscriptionInGooglePlayStore(Product oldProduct, Product newProduct, Action<string, string> googlePlayUpdateCallback) {
            SubscriptionManager oldSubscriptionManager = new SubscriptionManager(oldProduct, null);
            SubscriptionInfo oldSubscriptionInfo = null;
            try {
                oldSubscriptionInfo = oldSubscriptionManager.getSubscriptionInfo();
            } catch (Exception e) {
                Debug.unityLogger.Log("Error: the product that will be updated does not have a valid receipt", e);
                return;
            }
            string newSubscriptionId = newProduct.definition.storeSpecificId;
            googlePlayUpdateCallback(oldSubscriptionInfo.getSubscriptionInfoJsonString(), newSubscriptionId);
        }

        public static void UpdateSubscriptionInAppleStore(Product newProduct, string developerPayload, Action<Product, string> appleStoreUpdateCallback) {
            appleStoreUpdateCallback(newProduct, developerPayload);
        }

        // the receipt is Unity IAP UnifiedReceipt
        public SubscriptionManager(Product product, string intro_json) {
            this.receipt = product.receipt;
            this.productId = product.definition.storeSpecificId;
            this.intro_json = intro_json;
        }

        public SubscriptionManager(string receipt, string id, string intro_json) {
            this.receipt = receipt;
            this.productId = id;
            this.intro_json = intro_json;
        }

        // parse the "payload" part to get the subscription
        // info from the platform based native receipt
        public SubscriptionInfo getSubscriptionInfo() {
            if (this.receipt != null) {
				var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);

                var validPayload = receipt_wrapper.TryGetValue("Payload", out var payloadAsObject);
                var validStore  = receipt_wrapper.TryGetValue("Store", out var storeAsObject);
                if (validPayload && validStore)
                {
                    var payload = payloadAsObject as string;
                    var store = storeAsObject as string;
                    
                    switch (store) {
                        case GooglePlay.Name:
                        {
                            return getGooglePlayStoreSubInfo(payload);
                        }
                        case AppleAppStore.Name:
                        case MacAppStore.Name:
                        {
                            if (this.productId == null) {
                                throw new NullProductIdException();
                            }
                            return getAppleAppStoreSubInfo(payload, this.productId);
                        }
                        case AmazonApps.Name:
                        {
                            return getAmazonAppStoreSubInfo(payload, this.productId);
                        }
                        case HuaweiAppGallery.Name:
                        {
                            return SubscriptionInfo.CreateHuaweiSubscriptionInfo(payload, productId);
                        }
                        
                        #if UNITY_EDITOR
                        case "fake":
                        {
                            return new SubscriptionInfo(this.productId);
                        }
                        #endif
                        
                        default:
                        {
                            throw new StoreSubscriptionInfoNotSupportedException("Store not supported: " + store);
                        }
                    }
                }
            }

            #if UNITY_EDITOR
            if (FakeComputableSubscriptionInfo.HaveSubscriptionOfType(this.productId))
            {
                return new SubscriptionInfo(this.productId);
            }
            #endif
            
            throw new NullReceiptException();

        }

        private SubscriptionInfo getAmazonAppStoreSubInfo(string payload, string productId) {
            return new SubscriptionInfo(payload, productId);
        }
        private SubscriptionInfo getAppleAppStoreSubInfo(string payload, string productId) {

            AppleReceipt receipt = null;

            var logger = UnityEngine.Debug.unityLogger;

            try {
                receipt = new AppleReceiptParser().Parse(Convert.FromBase64String(payload));
            } catch (ArgumentException e) {
				logger.Log ("Unable to parse Apple receipt", e);
            } catch (IAPSecurityException e) {
				logger.Log ("Unable to parse Apple receipt", e);
            } catch (NullReferenceException e) {
				logger.Log ("Unable to parse Apple receipt", e);
            }

            List<AppleInAppPurchaseReceipt> inAppPurchaseReceipts = new List<AppleInAppPurchaseReceipt>();

            if (receipt != null && receipt.inAppPurchaseReceipts != null && receipt.inAppPurchaseReceipts.Length > 0) {
                foreach (AppleInAppPurchaseReceipt r in receipt.inAppPurchaseReceipts) {
                    if (r.productID.Equals(productId)) {
                        inAppPurchaseReceipts.Add(r);
                    }
                }
            }
            return inAppPurchaseReceipts.Count == 0 ? null : new SubscriptionInfo(findMostRecentReceipt(inAppPurchaseReceipts), this.intro_json);
        }

        private AppleInAppPurchaseReceipt findMostRecentReceipt(List<AppleInAppPurchaseReceipt> receipts) {
            receipts.Sort((b, a) => (a.purchaseDate.CompareTo(b.purchaseDate)));
            return receipts[0];
        }

        private SubscriptionInfo getGooglePlayStoreSubInfo(string payload)
        {
            var payload_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
            var validSkuDetailsKey = payload_wrapper.TryGetValue("skuDetails", out var skuDetailsObject);

            string skuDetails = null;
            if (validSkuDetailsKey) skuDetails = skuDetailsObject as string;

            var purchaseHistorySupported = false;

            var original_json_payload_wrapper =
                (Dictionary<string, object>) MiniJson.JsonDecode((string) payload_wrapper["json"]);

            var validIsAutoRenewingKey =
                original_json_payload_wrapper.TryGetValue("autoRenewing", out var autoRenewingObject);

            var isAutoRenewing = false;
            if (validIsAutoRenewingKey) isAutoRenewing = (bool) autoRenewingObject;

            // Google specifies times in milliseconds since 1970.
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var validPurchaseTimeKey =
                original_json_payload_wrapper.TryGetValue("purchaseTime", out var purchaseTimeObject);

            long purchaseTime = 0;

            if (validPurchaseTimeKey) purchaseTime = (long) purchaseTimeObject;

            var purchaseDate = epoch.AddMilliseconds(purchaseTime);

            var validDeveloperPayloadKey =
                original_json_payload_wrapper.TryGetValue("developerPayload", out var developerPayloadObject);

            var isFreeTrial = false;
            var hasIntroductoryPrice = false;
            string updateMetadata = null;

            if (validDeveloperPayloadKey)
            {
                var developerPayloadJSON = (string) developerPayloadObject;
                var developerPayload_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(developerPayloadJSON);
                var validIsFreeTrialKey =
                    developerPayload_wrapper.TryGetValue("is_free_trial", out var isFreeTrialObject);
                if (validIsFreeTrialKey) isFreeTrial = (bool) isFreeTrialObject;

                var validHasIntroductoryPriceKey =
                    developerPayload_wrapper.TryGetValue("has_introductory_price_trial",
                        out var hasIntroductoryPriceObject);

                if (validHasIntroductoryPriceKey) hasIntroductoryPrice = (bool) hasIntroductoryPriceObject;

                var validIsUpdatedKey = developerPayload_wrapper.TryGetValue("is_updated", out var isUpdatedObject);

                var isUpdated = false;

                if (validIsUpdatedKey) isUpdated = (bool) isUpdatedObject;

                if (isUpdated)
                {
                    var isValidUpdateMetaKey = developerPayload_wrapper.TryGetValue("update_subscription_metadata",
                        out var updateMetadataObject);

                    if (isValidUpdateMetaKey) updateMetadata = (string) updateMetadataObject;
                }
            }

            return new SubscriptionInfo(skuDetails, isAutoRenewing, purchaseDate, isFreeTrial, hasIntroductoryPrice,
                purchaseHistorySupported, updateMetadata);
        }
    }
}
