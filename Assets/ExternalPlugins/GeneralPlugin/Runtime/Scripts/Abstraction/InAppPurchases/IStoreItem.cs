using System;


namespace Modules.General.Abstraction.InAppPurchase
{
    public interface IStoreItem
    {
        event Action DataReceived;
        event Func<IPurchaseItemResult, bool> PurchaseComplete;
        event Action<IPurchaseItemResult> PurchaseRestored;
        
        /// <summary>
        /// Gets a platform-independent identifier of the store item.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets a platform-depended product id.
        /// </summary>
        string ProductId { get; }

        /// <summary>
        /// Gets a platform-depended parent product id.
        /// Used for subscriptions on AmazonAppStore.
        /// </summary>
        string ParentProductId { get; }
        
        /// <summary>
        /// Gets whether has platform-depended parent product id.
        /// Used for subscriptions on AmazonAppStore.
        /// </summary>
        bool HasParentProduct { get; }
        
        /// <summary>
        /// Gets a tier that describes a price of the product.
        /// </summary>
        int Tier { get; }

        /// <summary>
        /// Gets a value of price in USD associated with <see cref="Tier"/>.
        /// </summary>
        float TierPrice { get; }
        
        /// <summary>
        /// Gets a status of the store item.
        /// </summary>
        StoreItemStatus Status { get; }

        /// <summary>
        /// Gets a string with price of the product in the local currency (with currency symbol).
        /// </summary>
        string LocalizedPrice { get; }
        
        /// <summary>
        /// Gets a string with price of the product in the local currency (without currency symbol).
        /// </summary>
        string RealPrice { get; }

        /// <summary>
        /// Gets a currency code assigned with price of the shop item.
        /// </summary>
        string CurrencyCode { get; }

        /// <summary>
        /// Gets a title of the shop item that defined on a store side.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets a description of the shop item that defined on a store side.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets whether the shop item is consumable product.
        /// </summary>
        bool IsConsumable { get; }

        /// <summary>
        /// Gets whether the shop item is non-consumable product.
        /// </summary>
        bool IsNonConsumable { get; }

        /// <summary>
        /// Gets whether the shop item is a subscription.
        /// </summary>
        bool IsSubscription { get; }
        
        /// <summary>
        /// Gets whether the product price is actual.
        /// </summary>
        bool IsPriceActual { get; }
        
        /// <summary>
        /// Gets whether the user owns this store item.
        /// <para>
        /// KEEP IN MIND! This property only works with non-consumable products and subscriptions.
        /// </para>
        /// <para>
        /// The property returns true following cases:
        /// 1) For non-consumable - if it ever has been purchased;
        /// 2) For subscription - if it has been purchased and is currently active;
        /// Otherwise - false.
        /// </para>
        /// </summary>
        bool IsPurchased { get; }
        
        
        /// <summary>
        /// Returns helper for retrieving subscription details. Returns null, if current item isn't a subscription.
        /// </summary>
        ISubscriptionInfo SubscriptionInfo { get; }
        
        
        /// <summary>
        /// Returns type of subscription. Returns null if current item isn't a subscription.
        /// </summary>
        SubscriptionType SubscriptionType { get; }
        
        
        /// <summary>
        /// Returns true if identifier of to store item is equal to specified value; otherwise, false.
        /// </summary>
        /// <param name="identifier">The value to compare.</param>
        /// <returns></returns>
        bool IsIdentifierEqualsTo(string identifier);
        
        
        /// <summary>
        /// Returns true if value of property <see cref="ProductId"/> is equal to specified value; otherwise, false.
        /// </summary>
        /// <param name="productId">The value to compare.</param>
        /// <returns></returns>
        bool IsProductIdEqualsTo(string productId);
        
        
        /// <summary>
        /// Requests a purchase operation with the store item.
        /// </summary>
        /// <param name="callback">Optional callback of the operation.</param>
        void Purchase(Func<IPurchaseItemResult, bool> callback = null);
    }
}
