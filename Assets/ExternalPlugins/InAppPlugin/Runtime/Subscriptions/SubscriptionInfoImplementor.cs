using Modules.General.Abstraction.InAppPurchase;
using System;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class SubscriptionInfoImplementor : ISubscriptionInfo
    {
        private SubscriptionInfo subscriptionInfo;
        private IStoreItem storeItem;


        public IStoreItem StoreItem => storeItem;


        public string ProductId => subscriptionInfo?.getProductId() ?? string.Empty;
        
        
        public DateTime PurchaseDate => subscriptionInfo?.getPurchaseDate() ?? DateTime.MaxValue;
        
        
        public DateTime ExpirationDate => subscriptionInfo?.getExpireDate() ?? DateTime.MaxValue;
        
        
        public bool IsSubscribed => subscriptionInfo != null && subscriptionInfo.isSubscribed() == Result.True;
        
        
        public bool IsExpired => subscriptionInfo == null || subscriptionInfo.isExpired() != Result.False;
        
        
        public bool IsCancelled => subscriptionInfo == null || subscriptionInfo.isCancelled() != Result.False;
        
        
        public bool IsFreeTrial => subscriptionInfo != null && subscriptionInfo.isFreeTrial() == Result.True;
        
        
        public bool IsAutoRenewing => subscriptionInfo != null && subscriptionInfo.isAutoRenewing() == Result.True;
        
        
        public TimeSpan RemainingTime => subscriptionInfo?.getRemainingTime() ?? TimeSpan.MaxValue;
        
        
        internal SubscriptionInfoImplementor(IStoreItem item, SubscriptionInfo info)
        {
            storeItem = item ?? throw new ArgumentNullException();
            subscriptionInfo = info;
        }
    }
}
