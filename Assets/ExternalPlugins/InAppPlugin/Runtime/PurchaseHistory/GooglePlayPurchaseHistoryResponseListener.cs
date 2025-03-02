using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.InAppPurchase
{
    /// <summary>
    /// This is C# representation of the Java Class PurchaseHistoryResponseListener
    /// <a href="https://developer.android.com/reference/com/android/billingclient/api/PurchaseHistoryResponseListener">See more</a>
    /// </summary>
    internal class GooglePlayPurchaseHistoryResponseListener : AndroidJavaProxy
    {
        private const string PurchaseHistoryResponseListenerClassName = "com.android.billingclient.api.PurchaseHistoryResponseListener";
        private Action<List<GooglePlayPurchaseHistoryRecord>> historyRecordsCallback;
        
        
        public GooglePlayPurchaseHistoryResponseListener(Action<List<GooglePlayPurchaseHistoryRecord>> callback) :
            base(PurchaseHistoryResponseListenerClassName)
        {
            historyRecordsCallback = callback;
        }
        
        
        // ReSharper disable once InconsistentNaming
        private void onPurchaseHistoryResponse(AndroidJavaObject billingResult, AndroidJavaObject purchaseHistoryRecordList)
        {
            if (historyRecordsCallback == null)
            {
                return;
            }
            
            List<GooglePlayPurchaseHistoryRecord> result = new List<GooglePlayPurchaseHistoryRecord>();
            if (billingResult.Call<int>("getResponseCode") == 0)
            {
                int size = purchaseHistoryRecordList.Call<int>("size");
                for (int i = 0; i < size; i++)
                {
                    AndroidJavaObject javaObject = purchaseHistoryRecordList.Call<AndroidJavaObject>("get", i);
                    result.Add(new GooglePlayPurchaseHistoryRecord(javaObject));
                }
            }
            
            historyRecordsCallback?.Invoke(result);
        }
    }
}
