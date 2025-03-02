using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    // https://docs.unity3d.com/Manual/UnityIAPPurchaseReceipts.html
    public class PurchaseReceiptParser
    {
        private const string StoreKey = "Store";
        private const string TransactionIdKey = "TransactionID";
        private const string PayloadKey = "Payload";
        private const string GooglePlayJsonKey = "json";
        private const string GooglePlaySignatureKey = "signature";
        private const string GooglePlayPurchaseTokenKey = "purchaseToken";
        private const string UserIdKey = "userId";
        private const string ReceiptIdKey = "receiptId";
        private const string SubscriptionId = "subscriptionId";
        private const string PurchaseToken = "purchaseToken";
        private const string PurchaseType = "purchaseType";
        
        private string googlePlayPurchaseToken = string.Empty;
        
        
        public string Store { get; } = string.Empty;
        public string TransactionId { get; } = string.Empty;
        public string Payload { get; } = string.Empty;
        public string GooglePlayJson { get; } = string.Empty;
        public string GooglePlaySignature { get; } = string.Empty;
        public string GooglePlayPurchaseToken
        {
            get
            {
                if (StoreUtilities.StoreType != AppStore.GooglePlay)
                {
                    return string.Empty;
                }
                
                if (string.IsNullOrEmpty(googlePlayPurchaseToken))
                {
                    Dictionary<string, object> payloadJsonContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(GooglePlayJson);
                    googlePlayPurchaseToken = payloadJsonContent.ContainsKey(GooglePlayPurchaseTokenKey) ?
                        (string)payloadJsonContent[GooglePlayPurchaseTokenKey] :
                        string.Empty;
                }
                
                return googlePlayPurchaseToken;
            }
        }
        public string AmazonUserId { get; } = string.Empty;
        public string AmazonReceiptId { get; } = string.Empty;
        public string HuaweiSubscriptionId { get; } = string.Empty;
        public string HuaweiPurchaseToken { get; } = string.Empty;
        public string HuaweiPurchaseType { get; } = string.Empty;


        internal PurchaseReceiptParser(string receipt)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                return;
            }
            
            Dictionary<string, object> mainJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(receipt);
            if (mainJson == null)
            {
                Debug.LogError($"Can't parse receipt {receipt}");
                return;
            }
            
            Store = mainJson.ContainsKey(StoreKey) ? (string)mainJson[StoreKey] : string.Empty;
            TransactionId = mainJson.ContainsKey(TransactionIdKey) ? (string)mainJson[TransactionIdKey] : string.Empty;
            Payload = mainJson.ContainsKey(PayloadKey) ? (string)mainJson[PayloadKey] : string.Empty;
            
            if (StoreUtilities.StoreType == AppStore.GooglePlay)
            {
                Dictionary<string, object> payloadContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(Payload);
                GooglePlayJson = payloadContent.ContainsKey(GooglePlayJsonKey) ? (string)payloadContent[GooglePlayJsonKey] : string.Empty;
                GooglePlaySignature = payloadContent.ContainsKey(GooglePlaySignatureKey) ? (string)payloadContent[GooglePlaySignatureKey] : string.Empty;
            }
            else if (StoreUtilities.StoreType == AppStore.AmazonAppStore)
            {
                Dictionary<string, object> payloadContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(Payload);
                AmazonUserId = payloadContent.ContainsKey(UserIdKey) ? (string)payloadContent[UserIdKey] : string.Empty;
                AmazonReceiptId = payloadContent.ContainsKey(ReceiptIdKey) ? (string)payloadContent[ReceiptIdKey] : string.Empty;
            }
            else if (StoreUtilities.StoreType == AppStore.HuaweiAppGallery)
            {
                if (string.IsNullOrEmpty(Payload))
                {
                    CustomDebug.LogError($"Empty payload for HuaweiAppGallery");
                    return;
                }
                Dictionary<string, object> payloadContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(Payload);
                if (payloadContent == null)
                {
                    CustomDebug.LogError($"Can't parse payload {Payload}");
                    return;
                }
                HuaweiSubscriptionId = payloadContent.ContainsKey(SubscriptionId) ? (string)payloadContent[SubscriptionId] : string.Empty;
                HuaweiPurchaseToken = payloadContent.ContainsKey(PurchaseToken) ? (string)payloadContent[PurchaseToken] : string.Empty;
                HuaweiPurchaseType = payloadContent.ContainsKey(PurchaseType) ? payloadContent[PurchaseType].ToString() : string.Empty;
            }
        }
    }
}
