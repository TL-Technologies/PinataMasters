using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Modules.InAppPurchase
{
    public class HuaweiComputableSubscriptionInfo : IComputableSubscriptionInfo
    {
        #region Fields

        private DateTime purchaseDate;
        
        private Result autoRenewing;
        
        private DateTime expireDate;

        private Result freeTrial;

        private DateTime cancellationDate;

        private Result introductoryPricePeriod;

        private Dictionary<string, object> payload;
        
        #endregion


        
        #region Properties

        public string PurchaseToken { get; }
        
        
        public string SubscriptionId { get; }

        #endregion



        #region Class lifecycle

        public HuaweiComputableSubscriptionInfo(string payloadString)
        {
            payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadString);

            purchaseDate = GetDateTimeFromTimestamp("purchaseTime");
            autoRenewing = TryGetFromPayload("isAutoRenewing").Equals("True") ? Result.True : Result.False;
            expireDate = GetDateTimeFromTimestamp("expirationDate");
            freeTrial = TryGetFromPayload("trialFlag").Equals(1) ? Result.True : Result.False;
            cancellationDate = GetDateTimeFromTimestamp("cancellationTime");
            introductoryPricePeriod = TryGetFromPayload("introductoryFlag").Equals(1) ? Result.True : Result.False;
            PurchaseToken = TryGetFromPayload("purchaseToken").ToString();
            SubscriptionId = TryGetFromPayload("subscriptionId").ToString();
        }
        
        #endregion
        
        
        
        #region Methods

        private DateTime GetDateTimeFromTimestamp(string key)
        {
            if (!long.TryParse(TryGetFromPayload(key).ToString(), out long millisecondsFromEpochLong))
            {
                CustomDebug.Log(
                    $"[HuaweiComputableSubscriptionInfo - GetDateTimeFromTimestamp] cant parse {key} to long");
                return default;
            }
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(millisecondsFromEpochLong);
        }
        

        private object TryGetFromPayload(string key)
        {
            if (payload.TryGetValue(key, out object obj))
            {
                return obj;
            }
            CustomDebug.LogError($"[HuaweiComputableSubscriptionInfo] Error getting {key} from payload");
            return null;
        }

        #endregion

        

        #region IComputableSubscriptionInfo

        public DateTime getPurchaseDate() => purchaseDate;
        

        public Result isAutoRenewing() => autoRenewing;


        public DateTime getExpireDate() => expireDate;
        

        public Result isFreeTrial() => freeTrial; 
        

        public Result isSubscribed()
        {
            return getExpireDate().Ticks >= StoreUtilities.VerifiedDateTime.Ticks ?
                Result.True :
                Result.False;
        }

        
        public Result isExpired()
        {
            DateTime expirationDate = getExpireDate();
            DateTime currentDate = StoreUtilities.VerifiedDateTime;
            return 0L < expirationDate.Ticks && expirationDate.Ticks < currentDate.Ticks ? Result.True : Result.False;
        }
        
        
        public Result isCancelled()
        {
            DateTime currentDate = StoreUtilities.VerifiedDateTime;
            return 0L < cancellationDate.Ticks && cancellationDate.Ticks < currentDate.Ticks ? Result.True : Result.False;
        }

        
        public TimeSpan getRemainingTime()
        {
            return isSubscribed() == Result.True ?
                getExpireDate().Subtract(StoreUtilities.VerifiedDateTime) :
                TimeSpan.Zero;
        }


        public Result isIntroductoryPricePeriod() => introductoryPricePeriod;

        #endregion
    }
}