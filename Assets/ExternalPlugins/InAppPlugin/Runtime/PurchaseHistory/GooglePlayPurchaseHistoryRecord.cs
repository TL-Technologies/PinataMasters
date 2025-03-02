using System;
using UnityEngine;


namespace Modules.InAppPurchase
{
    internal class GooglePlayPurchaseHistoryRecord
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        
        private AndroidJavaObject javaObject;
        
        
        public string DeveloperPayload => javaObject.Call<string>("getDeveloperPayload");
        public string OriginalJson => javaObject.Call<string>("getOriginalJson");
        public DateTime PurchaseDate => UnixEpoch.AddMilliseconds(javaObject.Call<long>("getPurchaseTime"));
        public string PurchaseToken => javaObject.Call<string>("getPurchaseToken");
        public string Signature => javaObject.Call<string>("getSignature");
        public string ProductId => javaObject.Call<string>("getSku");
        
        
        public GooglePlayPurchaseHistoryRecord(AndroidJavaObject androidJavaObject)
        {
            javaObject = androidJavaObject ?? throw new ArgumentNullException();
        }
    }
}
