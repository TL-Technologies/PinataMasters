using System;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


namespace Modules.InAppPurchase
{
    internal class AppleComputableSubscriptionInfo : IComputableSubscriptionInfo
    {
        private AppleInAppPurchaseReceipt purchaseReceipt;
        private AppleStoreProductType storeProductType;
        
        
        public AppleComputableSubscriptionInfo(
            AppleStoreProductType productType,
            AppleInAppPurchaseReceipt receipt)
        {
            storeProductType = productType;
            purchaseReceipt = receipt;
        }


        public DateTime getPurchaseDate() => purchaseReceipt.purchaseDate;


        public Result isSubscribed()
        {
            if (storeProductType == AppleStoreProductType.NonRenewingSubscription)
            {
                return Result.Unsupported;
            }
            
            return purchaseReceipt.subscriptionExpirationDate.Ticks >= StoreUtilities.VerifiedDateTime.Ticks ?
                Result.True :
                Result.False;
        }


        public Result isExpired()
        {
            DateTime expirationDate = purchaseReceipt.subscriptionExpirationDate;
            DateTime currentDate = StoreUtilities.VerifiedDateTime;
            return 0L < expirationDate.Ticks && expirationDate.Ticks < currentDate.Ticks ? Result.True : Result.False;
        }


        public Result isCancelled()
        {
            if (storeProductType == AppleStoreProductType.NonRenewingSubscription)
            {
                return Result.Unsupported;
            }
            
            DateTime cancellationDate = purchaseReceipt.cancellationDate;
            DateTime currentDate = StoreUtilities.VerifiedDateTime;
            return 0L < cancellationDate.Ticks && cancellationDate.Ticks < currentDate.Ticks ? Result.True : Result.False;
        }
        
        
        public Result isFreeTrial()
        {
            if (storeProductType == AppleStoreProductType.NonRenewingSubscription)
            {
                return Result.Unsupported;
            }
            
            return purchaseReceipt.isFreeTrial == 1 ? Result.True : Result.False;
        }


        public Result isAutoRenewing()
        {
            Result result;
            
            if (storeProductType == AppleStoreProductType.NonRenewingSubscription)
            {
                result = Result.Unsupported;
            }
            else if (storeProductType == AppleStoreProductType.AutoRenewingSubscription &&
                isCancelled() == Result.False &&
                isExpired() == Result.False)
            {
                result = Result.True;
            }
            else
            {
                result = Result.False;
            }
            
            return result;
        }


        public TimeSpan getRemainingTime()
        {
            return isSubscribed() == Result.True ?
                purchaseReceipt.subscriptionExpirationDate.Subtract(StoreUtilities.VerifiedDateTime) :
                TimeSpan.Zero;
        }


        public Result isIntroductoryPricePeriod()
        {
            if (storeProductType == AppleStoreProductType.NonRenewingSubscription)
            {
                return Result.Unsupported;
            }
            
            return purchaseReceipt.isIntroductoryPricePeriod == 1 ? Result.True : Result.False;
        }


        public DateTime getExpireDate() =>purchaseReceipt.subscriptionExpirationDate;
    }
}
