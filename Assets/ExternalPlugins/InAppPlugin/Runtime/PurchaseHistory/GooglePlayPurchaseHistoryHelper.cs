using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class GooglePlayPurchaseHistoryHelper : IPurchaseHistoryHelper
    {
        private Dictionary<string, GooglePlayPurchaseHistoryRecord> purchaseHistoryRecords = new Dictionary<string, GooglePlayPurchaseHistoryRecord>();
        
        
        public void Initialize(IExtensionProvider extensionProvider)
        {
            IGooglePlayStoreExtensions googlePlayStoreExtension = extensionProvider.GetExtension<IGooglePlayStoreExtensions>();
            object googlePlayStoreService = ReflectionHelper.GetFieldValue<object>(
                Type.GetType("UnityEngine.Purchasing.GooglePlayStoreExtensions, UnityEngine.Purchasing.Stores"),
                googlePlayStoreExtension,
                "m_GooglePlayStoreService");
            object billingClientService = ReflectionHelper.GetFieldValue<object>(
                Type.GetType("UnityEngine.Purchasing.GooglePlayStoreService, UnityEngine.Purchasing.Stores"),
                googlePlayStoreService,
                "m_BillingClient");
            AndroidJavaObject billingClient = ReflectionHelper.GetFieldValue<AndroidJavaObject>(
                Type.GetType("UnityEngine.Purchasing.Models.GoogleBillingClient, UnityEngine.Purchasing.Stores"),
                billingClientService,
                "m_BillingClient");
                
            billingClient.Call(
                "queryPurchaseHistoryAsync",
                "subs",
                new GooglePlayPurchaseHistoryResponseListener(ProcessHistoryRecords));
            billingClient.Call(
                "queryPurchaseHistoryAsync",
                "inapp",
                new GooglePlayPurchaseHistoryResponseListener(ProcessHistoryRecords));
        }


        public bool WasItemPurchased(IStoreItem storeItem) => purchaseHistoryRecords.ContainsKey(storeItem.ProductId);
        
        
        private void ProcessHistoryRecords(List<GooglePlayPurchaseHistoryRecord> records)
        {
            foreach (GooglePlayPurchaseHistoryRecord historyRecord in records)
            {
                purchaseHistoryRecords.Add(historyRecord.ProductId, historyRecord);
            }
        }
    }
}
