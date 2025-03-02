using System;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal interface IComputableSubscriptionInfo
    {
        DateTime getPurchaseDate();
        Result isSubscribed();
        Result isExpired();
        Result isCancelled();
        Result isFreeTrial();
        Result isAutoRenewing();
        TimeSpan getRemainingTime();
        Result isIntroductoryPricePeriod();
        DateTime getExpireDate();
    }
}
