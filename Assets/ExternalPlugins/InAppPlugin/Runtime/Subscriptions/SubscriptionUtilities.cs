using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Collections.Generic;


namespace Modules.InAppPurchase
{
    public static class SubscriptionUtilities
    {
        // This extension is copied from UnityEngine.Purchasing.SerializationExtensions
        internal static string TryGetString(this Dictionary<string, object> dic, string key) => 
            dic.ContainsKey(key) && dic[key] != null ? dic[key].ToString() : (string)null;
        
        
        public static List<ISubscriptionInfo> GetSubscriptionsForDate(
            this IStoreManager storeManager,
            DateTime dateTime)
        {
            List<ISubscriptionInfo> result = new List<ISubscriptionInfo>();
            StoreManager manager = storeManager as StoreManager;;
            foreach (IStoreItem storeItem in manager.StoreItems)
            {
                ISubscriptionInfo info = storeItem.SubscriptionInfo;
                if (info != null && info.PurchaseDate <= dateTime && dateTime <= info.ExpirationDate)
                {
                    result.Add(info);
                }
            }
            
            return result;
        }
        
        
        public static ISubscriptionInfo GetLastSubscription(this IStoreManager storeManager)
        {
            ISubscriptionInfo result = null;
            StoreManager manager = storeManager as StoreManager;;
            foreach (IStoreItem storeItem in manager.StoreItems)
            {
                ISubscriptionInfo info = storeItem.SubscriptionInfo;
                if (info != null && info.PurchaseDate > DateTime.MinValue)
                {
                    if (result == null || result.PurchaseDate < info.PurchaseDate)
                    {
                        result = info;
                    }
                }
            }
            
            return result;
        }
    }
}
