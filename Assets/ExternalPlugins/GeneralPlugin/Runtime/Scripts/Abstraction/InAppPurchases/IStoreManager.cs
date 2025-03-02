using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction.InAppPurchase
{
    public interface IStoreManager
    {
        event Action<IStoreItem> ItemDataReceived;
        event Func<IPurchaseItemResult, bool> PurchaseComplete;
        event Action<IRestorePurchasesResult> RestorePurchasesComplete;

        void PurchaseItem(IStoreItem item, Func<IPurchaseItemResult, bool> callback = null);
        void PurchaseItem(string identifier, Func<IPurchaseItemResult, bool> callback = null);
        
        bool IsRestorePurchasesImplemented { get; }
        void RestorePurchases(Action<IRestorePurchasesResult> callback = null);
        
        List<IStoreItem> StoreItems { get; }
        bool IsSandboxEnvironment { get; }
        IStoreItem GetStoreItem(string identifier);
        bool IsStoreItemPurchased(IStoreItem storeItem);
        bool IsStoreItemPurchased(string identifier);
        
        ISubscriptionRewardManager SubscriptionRewardManager { get; }

        bool IsFakeSubscriptionEnabled { get; set; }
        bool HasAnyActiveSubscription { get; }
        List<IStoreItem> GetActiveSubscriptions();
        bool IsSubscriptionTrialAvailable(IStoreItem item);
        bool IsSubscriptionTrialAvailable(string identifier);
        
        void ConfirmPendingPurchase(IPurchaseItemResult result);
        
        bool IsInitialized { get; }
    }
}
