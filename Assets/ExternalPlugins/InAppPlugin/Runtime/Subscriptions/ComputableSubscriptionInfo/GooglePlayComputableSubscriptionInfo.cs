using System;
using System.Xml;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class GooglePlayComputableSubscriptionInfo : IComputableSubscriptionInfo
    {
        #region Fields

        private bool isSubscriptionAutoRenewing;
        
        private DateTime purchaseDate;
        private TimeSpan updateMetadataPeriod;
        private TimeSpan introductoryTimePeriod;
        private TimeSpan freeTrialPeriod;
        
        private TimeSpanUnits introductoryPricePeriod;
        private TimeSpanUnits subscriptionPeriod;

        #endregion



        #region Class lifecycle

        internal GooglePlayComputableSubscriptionInfo(
            bool isSubscriptionAutoRenewing,
            DateTime purchaseDate,
            TimeSpan updateMetadataPeriod,
            TimeSpan introductoryTimePeriod,
            TimeSpan freeTrialPeriod,
            string introductoryPricePeriod,
            string subscriptionPeriod)
        {
            this.isSubscriptionAutoRenewing = isSubscriptionAutoRenewing;
            this.purchaseDate = purchaseDate;
            this.updateMetadataPeriod = updateMetadataPeriod;
            this.introductoryTimePeriod = introductoryTimePeriod;
            this.freeTrialPeriod = freeTrialPeriod;
            this.introductoryPricePeriod = parsePeriodTimeSpanUnits(introductoryPricePeriod);
            this.subscriptionPeriod = parsePeriodTimeSpanUnits(subscriptionPeriod);
        }

        #endregion
        


        #region IComputableSubscriptionInfo

        public DateTime getPurchaseDate() => purchaseDate;


        public Result isSubscribed() => Result.True;


        public Result isExpired() => Result.False;


        public Result isCancelled() => isSubscriptionAutoRenewing ? Result.False : Result.True;
        
        
        public Result isFreeTrial()
        {
            Result result = Result.False;
            TimeSpan timeSpan = StoreUtilities.VerifiedDateTime.Subtract(purchaseDate);
            
            if (updateMetadataPeriod < timeSpan && timeSpan <= freeTrialPeriod.Add(updateMetadataPeriod))
            {
                result = Result.True;
            }
            
            return result;
        }


        public Result isAutoRenewing() => isSubscriptionAutoRenewing ? Result.True : Result.False;


        public TimeSpan getRemainingTime() => getExpireDate().Subtract(StoreUtilities.VerifiedDateTime);


        public Result isIntroductoryPricePeriod()
        {
            Result result = Result.False;
            TimeSpan timeSpan = StoreUtilities.VerifiedDateTime.Subtract(purchaseDate);
            
            if (freeTrialPeriod.Add(updateMetadataPeriod) < timeSpan &&
                timeSpan < freeTrialPeriod.Add(updateMetadataPeriod).Add(introductoryTimePeriod))
            {
                result = Result.True;
            }
            
            return result;
        }
        
        
        public DateTime getExpireDate()
        {
            DateTime subscriptionExpireDate;
            
            TimeSpan timeSpan = StoreUtilities.VerifiedDateTime.Subtract(purchaseDate);
            if (timeSpan <= updateMetadataPeriod)
            {
                subscriptionExpireDate = purchaseDate.Add(updateMetadataPeriod);
            }
            else if (timeSpan <= freeTrialPeriod.Add(updateMetadataPeriod))
            {
                subscriptionExpireDate = purchaseDate.Add(this.freeTrialPeriod.Add(updateMetadataPeriod));
            }
            else if (timeSpan < freeTrialPeriod.Add(updateMetadataPeriod).Add(introductoryTimePeriod))
            {
                subscriptionExpireDate = nextBillingDate(
                    purchaseDate.Add(freeTrialPeriod.Add(updateMetadataPeriod)),
                    introductoryPricePeriod);
            }
            else
            {
                subscriptionExpireDate = nextBillingDate(
                    purchaseDate.Add(freeTrialPeriod.Add(updateMetadataPeriod).Add(introductoryTimePeriod)),
                    subscriptionPeriod);
            }
            
            return subscriptionExpireDate;
        }

        #endregion

        

        // Methods in this region are copied from UnityEngine.Purchasing.SubscriptionInfo class.
        // Code formatting remains similar to original code for the next updates alleviating.
        #region Subscription info methods
        
        private DateTime nextBillingDate(DateTime billing_begin_date, TimeSpanUnits units) {

            if (units.days == 0.0 && units.months == 0 && units.years == 0) return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime next_billing_date = billing_begin_date;
            // find the next billing date that after the current date
            while (DateTime.Compare(next_billing_date, StoreUtilities.VerifiedDateTime) <= 0) {

                next_billing_date = next_billing_date.AddDays(units.days).AddMonths(units.months).AddYears(units.years);
            }
            return next_billing_date;
        }
        
        
        private TimeSpan parseTimeSpan(string period_string) {
            TimeSpan result = TimeSpan.Zero;
            try {
                result = XmlConvert.ToTimeSpan(period_string);
            } catch(Exception) {
                if (period_string == null || period_string.Length == 0) {
                    result = TimeSpan.Zero;
                } else {
                    // .Net "P1W" is not supported and throws a FormatException
                    // not sure if only weekly billing contains "W"
                    // need more testing
                    result = new TimeSpan(7, 0, 0, 0);
                }
            }
            return result;
        }
        
        
        private TimeSpanUnits parsePeriodTimeSpanUnits(string time_span) {
            switch (time_span) {
                case "P1W":
                    // weekly subscription
                    return new TimeSpanUnits(7.0, 0, 0);
                case "P1M":
                    // monthly subscription
                    return new TimeSpanUnits(0.0, 1, 0);
                case "P3M":
                    // 3 months subscription
                    return new TimeSpanUnits(0.0, 3, 0);
                case "P6M":
                    // 6 months subscription
                    return new TimeSpanUnits(0.0, 6, 0);
                case "P1Y":
                    // yearly subscription
                    return new TimeSpanUnits(0.0, 0, 1);
                default:
                    // seasonal subscription or duration in days
                    return new TimeSpanUnits((double)parseTimeSpan(time_span).Days, 0, 0);
            }
        }
        
        #endregion
    }
}
