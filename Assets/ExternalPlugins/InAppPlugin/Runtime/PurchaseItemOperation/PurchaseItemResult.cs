using Modules.General.Abstraction.InAppPurchase;
using System;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


namespace Modules.InAppPurchase
{
    public class PurchaseItemResult : IPurchaseItemResult
    {
        #region Fields

        private static AppleReceiptParser appleReceiptParser;

        #endregion



        #region Properties

        public PurchaseItemResultCode ResultCode { get; }
        public string Message { get; }
        public IStoreItem StoreItem { get; }
        public string TransactionId { get; }
        public PurchaseTransactionState TransactionState { get; }
        public PurchaseValidationState ValidationState { get; }

        public bool IsSucceeded => ResultCode == PurchaseItemResultCode.Ok;
        public bool IsValidated => ValidationState == PurchaseValidationState.Valid;
        
        public PurchaseReceiptParser ReceiptParser { get; }

        private AppleReceiptParser AppleReceiptParser =>
            appleReceiptParser ?? (appleReceiptParser = new AppleReceiptParser());

        #endregion



        #region Class lifecycle
        
        internal PurchaseItemResult(
            IStoreItem storeItem,
            PurchaseItemResultCode resultCode,
            string message = null)
        {
            StoreItem = storeItem;
            ResultCode = resultCode;
            Message = message;
            ReceiptParser = new PurchaseReceiptParser(null);
        }

        
        internal PurchaseItemResult(
            IStoreItem storeItem,
            Product purchasedProduct,
            PurchaseTransactionState transactionState,
            PurchaseValidationState validationState)
        {
            StoreItem item = storeItem as StoreItem;
            StoreItem = storeItem;
        
            if (transactionState == PurchaseTransactionState.Failed)
            {
                ResultCode = PurchaseItemResultCode.Failed;
            }
            else if (validationState != PurchaseValidationState.Valid)
            {
                ResultCode = PurchaseItemResultCode.ValidationFailed;
            }
            else
            {
                ResultCode = PurchaseItemResultCode.Ok;
            }
        
            TransactionState = transactionState;
            ValidationState = validationState;
            
            ReceiptParser = new PurchaseReceiptParser(purchasedProduct.receipt);
            string originalTransactionId = GetOriginalTransactionId();
            if (!string.IsNullOrEmpty(originalTransactionId))
            {
                TransactionId = originalTransactionId;

                // TransactionID is the same as originalTransactionId only in case of a consumable item,
                // the first purchase of a non-consumable item, and the first purchase of a subscription item.
                // Hack: subscription check is required to send an event after purchasing a subscription. 
                if (TransactionState == PurchaseTransactionState.Purchased &&
                    item.Product.transactionID != originalTransactionId && !item.IsSubscription)
                {
                    TransactionState = PurchaseTransactionState.RePurchased;
                }
            }
            else
            {
                TransactionId = item.Product.transactionID;
            }
            
            
            string GetOriginalTransactionId()
            {
                string result = string.Empty;
                
                AppStore storeType = StoreUtilities.StoreType;
                if (storeType == AppStore.AppleAppStore || storeType == AppStore.MacAppStore)
                {
                    AppleReceipt appleReceipt = null;
                    try
                    {
                        appleReceipt = AppleReceiptParser.Parse(Convert.FromBase64String(ReceiptParser.Payload));
                    }
                    catch
                    {
                        // Ignored
                    }

                    if (appleReceipt != null)
                    {
                        foreach (AppleInAppPurchaseReceipt purchaseReceipt in appleReceipt.inAppPurchaseReceipts)
                        {
                            if (StoreItem.IsProductIdEqualsTo(purchaseReceipt.productID))
                            {
                                result = purchaseReceipt.originalTransactionIdentifier;
                                break;
                            }
                        }
                    }
                }
                
                return result;
            }
        }
        
        #endregion
    }
}
