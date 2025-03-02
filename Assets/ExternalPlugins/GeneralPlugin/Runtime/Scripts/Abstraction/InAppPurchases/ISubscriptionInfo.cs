using System;


namespace Modules.General.Abstraction.InAppPurchase
{
    public interface ISubscriptionInfo
    {
        IStoreItem StoreItem { get; }
        string ProductId { get; }
        DateTime PurchaseDate { get; }
        DateTime ExpirationDate { get; }
        bool IsSubscribed { get; }
        bool IsExpired { get; }
        bool IsCancelled { get; }
        bool IsFreeTrial { get; }
        bool IsAutoRenewing { get; }
        TimeSpan RemainingTime { get; }
    }
}
