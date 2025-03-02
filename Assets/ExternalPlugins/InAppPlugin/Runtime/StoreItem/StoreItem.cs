using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class StoreItem : IStoreItem
    {
        #region Fields

        public event Action DataReceived;
        public event Func<IPurchaseItemResult, bool> PurchaseComplete;
        public event Action<IPurchaseItemResult> PurchaseRestored;

        private Product product;
        private Product parentProduct;
        private ISubscriptionInfo subscriptionInfo;
        private IStoreManager storeManager;

        #endregion



        #region Properties

        /// <inheritdoc />
        public string Identifier { get; }
        
        
        /// <inheritdoc />
        public string ProductId { get; }

        
        /// <inheritdoc />
        public string ParentProductId { get; }


        /// <inheritdoc />
        public bool HasParentProduct => !string.IsNullOrEmpty(ParentProductId);
        
        
        /// <inheritdoc />
        public int Tier { get; }

        
        /// <inheritdoc />
        public float TierPrice { get; }
        
        
        /// <inheritdoc />
        public StoreItemStatus Status { get; private set; } = StoreItemStatus.Outdated;

        
        /// <inheritdoc />
        public string LocalizedPrice => product != null ? product.metadata.localizedPriceString : string.Empty;


        /// <inheritdoc />
        public string RealPrice => product != null
            ? Decimal.Round(product.metadata.localizedPrice, 2).ToString(CultureInfo.InvariantCulture)
            : string.Empty;


        /// <inheritdoc />
        public string CurrencyCode => product != null ? product.metadata.isoCurrencyCode : string.Empty;

        
        /// <inheritdoc />
        public string Title => product != null ? product.metadata.localizedTitle : string.Empty;

        
        /// <inheritdoc />
        public string Description => product != null ? product.metadata.localizedDescription : string.Empty;

        
        /// <inheritdoc />
        public bool IsConsumable => ItemType == ProductType.Consumable;

        
        /// <inheritdoc />
        public bool IsNonConsumable => ItemType == ProductType.NonConsumable;

        
        /// <inheritdoc />
        public bool IsSubscription => ItemType == ProductType.Subscription;
        
        
        /// <inheritdoc />
        public bool IsPriceActual => Status == StoreItemStatus.Actual;
        
        
        /// <inheritdoc />
        public bool IsPurchased => storeManager.IsStoreItemPurchased(this);
        
        
        /// <inheritdoc />
        public ISubscriptionInfo SubscriptionInfo => subscriptionInfo;
        
        
        /// <inheritdoc />
        public SubscriptionType SubscriptionType { get; }


        /// <summary>
        /// Gets a type of the store item.
        /// </summary>
        internal ProductType ItemType { get; }
        
        
        internal Product Product => product;

        
        internal Product ParentProduct => parentProduct;

        #endregion



        #region Instancing

        public StoreItem(StoreItemSettings settings, IStoreManager manager)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            
            Identifier = settings.id ?? throw new ArgumentNullException(nameof(settings.id));
            ProductId = GetProductId(settings);
            ItemType = settings.itemType;
            Tier = settings.priceTier;
            TierPrice = StoreUtilities.GetTierPrice(Tier);
            SubscriptionType = settings.subscriptionType;
            
            storeManager = manager;

            if (StoreUtilities.StoreType == AppStore.AmazonAppStore)
            {
                ParentProductId = GetParentProductId(settings);
            }
            
            #if UNITY_EDITOR
                StoreUtilities.CheckStoreItemSettings(settings);
            #endif
        }

        #endregion



        #region Methods
        
        /// <inheritdoc />
        public bool IsIdentifierEqualsTo(string identifier)
        {
            bool result = string.Equals(Identifier, identifier, StringComparison.Ordinal);
            
            if (!result && HasParentProduct)
            {
                result = string.Equals(ParentProductId.Split('.').Last(), identifier, StringComparison.Ordinal);
            }
            
            return result;
        }
        
        
        
        /// <inheritdoc />
        public bool IsProductIdEqualsTo(string productId)
        {
            bool result = string.Equals(ProductId, productId, StringComparison.Ordinal);
            
            if (!result && HasParentProduct)
            {
                result = string.Equals(ParentProductId, productId, StringComparison.Ordinal);
            }
            
            return result;
        }
            
        
        
        /// <inheritdoc />
        public void Purchase(Func<IPurchaseItemResult, bool> callback = null) =>
            storeManager.PurchaseItem(this, callback);
        
        
        private string GetProductId(StoreItemSettings settings)
        {
            AppStore storeType = StoreUtilities.StoreType;
            StoreIdInfo storeIdInfo = settings.storeSpecificIds?.Find(info => info.storeType == storeType);
            
            string result;
            if (storeIdInfo != null)
            {
                result = storeIdInfo.isFullId ?
                    storeIdInfo.storeId :
                    $"{Application.identifier}.iap.{storeIdInfo.storeId}";
            }
            else
            {
                result = $"{Application.identifier}.iap.{settings.id}";
            }
            
            return result;
        }

        private string GetParentProductId(StoreItemSettings settings)
        {
            AppStore storeType = StoreUtilities.StoreType;
            StoreIdInfo storeIdInfo = settings.storeSpecificIds?.Find(info => info.storeType == storeType);
            
            string result = null;
            if (storeIdInfo != null && !string.IsNullOrEmpty(storeIdInfo.parentId))
            {
                result = storeIdInfo.isFullId ?
                    storeIdInfo.parentId :
                    $"{Application.identifier}.iap.{storeIdInfo.parentId}";
            }
            
            return result;
        }
        
        #endregion



        #region Events

        internal void InvokeDataReceived(Product receivedProduct, ISubscriptionInfo info, Product receivedParentProduct = null)
        {
            Status = StoreItemStatus.Actual;
            product = receivedProduct;
            subscriptionInfo = info;
            parentProduct = receivedParentProduct;
            
            OnDataReceived(product);
            DataReceived?.Invoke();
        }


        protected virtual void OnDataReceived(Product receivedProduct) { }
        
        
        internal bool InvokePurchaseComplete(IPurchaseItemResult purchaseItemResult, ISubscriptionInfo info)
        {
            subscriptionInfo = info;
            
            bool isRewardGranted = OnPurchaseComplete(purchaseItemResult);
            if (PurchaseComplete != null)
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (Func<IPurchaseItemResult, bool> func in PurchaseComplete.GetInvocationList())
                {
                    isRewardGranted |= func(purchaseItemResult);
                }
            }
            
            return isRewardGranted;
        }

        
        protected virtual bool OnPurchaseComplete(IPurchaseItemResult purchaseItemResult) => false;


        internal void InvokePurchaseRestored(IPurchaseItemResult purchaseItemResult, ISubscriptionInfo info)
        {
            subscriptionInfo = info;
            
            OnPurchaseRestored(purchaseItemResult);
            PurchaseRestored?.Invoke(purchaseItemResult);
        }

        
        protected virtual void OnPurchaseRestored(IPurchaseItemResult purchaseItemResult) { }

        #endregion
    }
}
