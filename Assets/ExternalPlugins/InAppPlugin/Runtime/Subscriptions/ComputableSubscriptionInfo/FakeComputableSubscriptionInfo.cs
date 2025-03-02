using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class FakeComputableSubscriptionInfo : IComputableSubscriptionInfo
    {
        private const int DurationMinutes = 10;
        private DateTime purchaseDate;
        
        
        public FakeComputableSubscriptionInfo(string productId)
        {
            if (HaveSubscriptionOfType(productId) &&
                DateTime.TryParse(PlayerPrefs.GetString($"fake_subscription_{productId}"), out purchaseDate))
            {
                return;
            }
            purchaseDate = DateTime.UtcNow;
            PlayerPrefs.SetString($"fake_subscription_{productId}", purchaseDate.ToString(CultureInfo.InvariantCulture));
        }
        
        
        public FakeComputableSubscriptionInfo(DateTime purchaseDate, string productId)
        {
            this.purchaseDate = purchaseDate;
        }
        
        
        public DateTime getPurchaseDate() => purchaseDate;


        public Result isSubscribed() => getExpireDate() > DateTime.UtcNow ? Result.True : Result.False;


        public Result isExpired() => getExpireDate() < DateTime.UtcNow ? Result.True : Result.False;


        public Result isCancelled() => Result.Unsupported;
        
        
        public Result isFreeTrial() => Result.Unsupported;


        public Result isAutoRenewing() => Result.Unsupported;


        public TimeSpan getRemainingTime() => getExpireDate() - DateTime.UtcNow;


        public Result isIntroductoryPricePeriod() => Result.Unsupported;


        public DateTime getExpireDate() => purchaseDate.AddMinutes(DurationMinutes);


        public static bool HaveSubscriptionOfType(string productId)
        {
            if (PlayerPrefs.HasKey($"fake_subscription_{productId}"))
            {
                if (DateTime.TryParse(PlayerPrefs.GetString($"fake_subscription_{productId}"),
                    out var purchaseDate))
                {
                    return purchaseDate.AddMinutes(DurationMinutes) > DateTime.UtcNow;
                }
            }
            return false;
        }
    }
}
