using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
// ReSharper disable InconsistentNaming
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable


namespace Modules.InAppPurchase
{
    // This class duplicates class UnityEngine.Purchasing.SubscriptionInfo, but:
    // 1. Replaces non-constant values by calculated ones (i.e. isSubscribed() is calculated during
    //    method call with actual DateTime value instead of calculating during constructor call.
    // 2. Replaces insecure calls to DateTime.UtcNow, which is vulnerable to time cheating,
    //    by using DateTime.UtcNow from server call.
    // Code formatting remains similar to original code for the next updates alleviating.
    internal class SubscriptionInfo
    {
        private DateTime VerifiedDateTime => StoreUtilities.VerifiedDateTime;
        //////////////////////////////////////////////////
        private DateTime purchaseDate;
        private Result is_subscribed;
        private Result is_expired;
        private Result is_cancelled;
        private Result is_free_trial;
        private Result is_auto_renewing;
        private TimeSpan remainedTime;
        private Result is_introductory_price_period;
        private DateTime subscriptionExpireDate;
        
        private string productId;
        private DateTime subscriptionCancelDate;
        private string introductory_price;
        private TimeSpan introductory_price_period;
        private long introductory_price_cycles;
        private TimeSpan freeTrialPeriod;
        private TimeSpan subscriptionPeriod;
        private IComputableSubscriptionInfo computableSubscriptionInfo;
        
        // for test
        private string free_trial_period_string;
        private string sku_details;
        
        
        public SubscriptionInfo() {}
        
        
        public SubscriptionInfo(AppleInAppPurchaseReceipt r, string intro_json)
        {
            var productType = (AppleStoreProductType) Enum.Parse(typeof(AppleStoreProductType), r.productType.ToString());

            if (productType == AppleStoreProductType.Consumable || productType == AppleStoreProductType.NonConsumable) {
                throw new InvalidProductTypeException();
            }

            if (!string.IsNullOrEmpty(intro_json)) {
                var intro_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(intro_json);
                var nunit = -1;
                var unit = SubscriptionPeriodUnit.NotAvailable;
                this.introductory_price = intro_wrapper.TryGetString("introductoryPrice") + intro_wrapper.TryGetString("introductoryPriceLocale");
                if (string.IsNullOrEmpty(this.introductory_price)) {
                    this.introductory_price = "not available";
                } else {
                    try {
                        this.introductory_price_cycles = Convert.ToInt64(intro_wrapper.TryGetString("introductoryPriceNumberOfPeriods"));
                        nunit = Convert.ToInt32(intro_wrapper.TryGetString("numberOfUnits"));
                        unit = (SubscriptionPeriodUnit)Convert.ToInt32(intro_wrapper.TryGetString("unit"));
                    } catch(Exception e) {
                        Debug.unityLogger.Log ("Unable to parse introductory period cycles and duration, this product does not have configuration of introductory price period", e);
                        unit = SubscriptionPeriodUnit.NotAvailable;
                    }
                }
                DateTime now = DateTime.Now;
                switch (unit) {
                    case SubscriptionPeriodUnit.Day:
                        this.introductory_price_period = TimeSpan.FromTicks(TimeSpan.FromDays(1).Ticks * nunit);
                        break;
                    case SubscriptionPeriodUnit.Month:
                        TimeSpan month_span = now.AddMonths(1) - now;
                        this.introductory_price_period = TimeSpan.FromTicks(month_span.Ticks * nunit);
                        break;
                    case SubscriptionPeriodUnit.Week:
                        this.introductory_price_period = TimeSpan.FromTicks(TimeSpan.FromDays(7).Ticks * nunit);
                        break;
                    case SubscriptionPeriodUnit.Year:
                        TimeSpan year_span = now.AddYears(1) - now;
                        this.introductory_price_period = TimeSpan.FromTicks(year_span.Ticks * nunit);
                        break;
                    case SubscriptionPeriodUnit.NotAvailable:
                        this.introductory_price_period = TimeSpan.Zero;
                        this.introductory_price_cycles = 0;
                        break;
                }
            } else {
                this.introductory_price = "not available";
                this.introductory_price_period = TimeSpan.Zero;
                this.introductory_price_cycles = 0;
            }

            DateTime current_date = VerifiedDateTime;
            this.purchaseDate = r.purchaseDate;
            this.productId = r.productID;

            this.subscriptionExpireDate = r.subscriptionExpirationDate;
            this.subscriptionCancelDate = r.cancellationDate;

            // if the product is non-renewing subscription, apple store will not return expiration date for this product
            if (productType == AppleStoreProductType.NonRenewingSubscription) {
                this.is_subscribed = Result.Unsupported;
                this.is_expired = Result.Unsupported;
                this.is_cancelled = Result.Unsupported;
                this.is_free_trial = Result.Unsupported;
                this.is_auto_renewing = Result.Unsupported;
                this.is_introductory_price_period = Result.Unsupported;
            } else {
                this.is_cancelled = (r.cancellationDate.Ticks > 0) && (r.cancellationDate.Ticks < current_date.Ticks) ? Result.True : Result.False;
                this.is_subscribed = r.subscriptionExpirationDate.Ticks >= current_date.Ticks ? Result.True : Result.False;
                this.is_expired = (r.subscriptionExpirationDate.Ticks > 0 && r.subscriptionExpirationDate.Ticks < current_date.Ticks) ? Result.True : Result.False;
                this.is_free_trial = (r.isFreeTrial == 1) ? Result.True : Result.False;
                this.is_auto_renewing = ((productType == AppleStoreProductType.AutoRenewingSubscription) && this.is_cancelled == Result.False
                        && this.is_expired == Result.False) ? Result.True : Result.False;
                this.is_introductory_price_period = r.isIntroductoryPricePeriod == 1 ? Result.True : Result.False;
            }

            if (this.is_subscribed == Result.True) {
                this.remainedTime = r.subscriptionExpirationDate.Subtract(current_date);
            } else {
                this.remainedTime = TimeSpan.Zero;
            }
            computableSubscriptionInfo = new AppleComputableSubscriptionInfo(productType, r);
        }

        public SubscriptionInfo(string skuDetails, bool isAutoRenewing, DateTime purchaseDate, bool isFreeTrial,
                                bool hasIntroductoryPriceTrial, bool purchaseHistorySupported, string updateMetadata)
        {
            var skuDetails_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(skuDetails);
            var validTypeKey = skuDetails_wrapper.TryGetValue("type", out var typeObject);

            if (!validTypeKey || (string)typeObject == "inapp") {
                throw new InvalidProductTypeException();
            }

            var validProductIdKey = skuDetails_wrapper.TryGetValue("productId", out var productIdObject);
            productId = null;
            if (validProductIdKey) productId = productIdObject as string;

            this.purchaseDate = purchaseDate;
            this.is_subscribed = Result.True;
            this.is_auto_renewing = isAutoRenewing ? Result.True : Result.False;
            this.is_expired = Result.False;
            this.is_cancelled = isAutoRenewing ? Result.False : Result.True;
            this.is_free_trial = Result.False;


            string sub_period = null;
            if (skuDetails_wrapper.ContainsKey("subscriptionPeriod")) {
                sub_period = (string)skuDetails_wrapper["subscriptionPeriod"];
            }
            string free_trial_period = null;
            if (skuDetails_wrapper.ContainsKey("freeTrialPeriod")) {
                free_trial_period = (string)skuDetails_wrapper["freeTrialPeriod"];
            }
            string introductory_price = null;
            if (skuDetails_wrapper.ContainsKey("introductoryPrice")) {
                introductory_price = (string)skuDetails_wrapper["introductoryPrice"];
            }
            string introductory_price_period_string = null;
            if (skuDetails_wrapper.ContainsKey("introductoryPricePeriod")) {
                introductory_price_period_string = (string)skuDetails_wrapper["introductoryPricePeriod"];
            }
            long introductory_price_cycles = 0;
            if (skuDetails_wrapper.ContainsKey("introductoryPriceCycles")) {
                introductory_price_cycles = (long)skuDetails_wrapper["introductoryPriceCycles"];
            }

            // for test
            free_trial_period_string = free_trial_period;

            this.subscriptionPeriod = computePeriodTimeSpan(parsePeriodTimeSpanUnits(sub_period));

            this.freeTrialPeriod = TimeSpan.Zero;
            if (isFreeTrial) {
                this.freeTrialPeriod = parseTimeSpan(free_trial_period);
            }

            this.introductory_price = introductory_price;
            this.introductory_price_cycles = introductory_price_cycles;
            this.introductory_price_period = TimeSpan.Zero;
            this.is_introductory_price_period = Result.False;
            TimeSpan total_introductory_duration = TimeSpan.Zero;

            if (hasIntroductoryPriceTrial) {
                if (introductory_price_period_string != null && introductory_price_period_string.Equals(sub_period)) {
                    this.introductory_price_period = this.subscriptionPeriod;
                } else {
                    this.introductory_price_period = parseTimeSpan(introductory_price_period_string);
                }
                // compute the total introductory duration according to the introductory price period and period cycles
                total_introductory_duration = accumulateIntroductoryDuration(parsePeriodTimeSpanUnits(introductory_price_period_string), this.introductory_price_cycles);
            }

            // if this subscription is updated from other subscription, the remaining time will be applied to this subscription
            TimeSpan extra_time = TimeSpan.FromSeconds(updateMetadata == null ? 0.0 : computeExtraTime(updateMetadata, this.subscriptionPeriod.TotalSeconds));

            TimeSpan time_since_purchased = VerifiedDateTime.Subtract(purchaseDate);


            // this subscription is still in the extra time (the time left by the previous subscription when updated to the current one)
            if (time_since_purchased <= extra_time) {
                // this subscription is in the remaining credits from the previous updated one
                this.subscriptionExpireDate = purchaseDate.Add(extra_time);
            } else if (time_since_purchased <= this.freeTrialPeriod.Add(extra_time)) {
                // this subscription is in the free trial period
                // this product will be valid until free trial ends, the beginning of next billing date
                this.is_free_trial = Result.True;
                this.subscriptionExpireDate = purchaseDate.Add(this.freeTrialPeriod.Add(extra_time));
            } else if (time_since_purchased < this.freeTrialPeriod.Add(extra_time).Add(total_introductory_duration)) {
                // this subscription is in the introductory price period
                this.is_introductory_price_period = Result.True;
                DateTime introductory_price_begin_date = this.purchaseDate.Add(this.freeTrialPeriod.Add(extra_time));
                this.subscriptionExpireDate = nextBillingDate(introductory_price_begin_date, parsePeriodTimeSpanUnits(introductory_price_period_string));
            } else {
                // no matter sub is cancelled or not, the expire date will be next billing date
                DateTime billing_begin_date = this.purchaseDate.Add(this.freeTrialPeriod.Add(extra_time).Add(total_introductory_duration));
                this.subscriptionExpireDate = nextBillingDate(billing_begin_date, parsePeriodTimeSpanUnits(sub_period));
            }

            this.remainedTime = this.subscriptionExpireDate.Subtract(VerifiedDateTime);
            this.sku_details = skuDetails;
      
            computableSubscriptionInfo = new GooglePlayComputableSubscriptionInfo(
                isAutoRenewing,
                purchaseDate,
                extra_time,
                total_introductory_duration,
                freeTrialPeriod,
                introductory_price_period_string,
                sub_period);
        }
        
        public SubscriptionInfo(string payload, string productId) {
            this.productId = productId;
            this.is_subscribed = Result.True;
            this.is_expired = Result.False;
            this.is_cancelled = Result.Unsupported;
            this.is_free_trial = Result.Unsupported;
            this.is_auto_renewing = Result.Unsupported;
            this.remainedTime = TimeSpan.MaxValue;
            this.is_introductory_price_period = Result.Unsupported;
            this.introductory_price_period = TimeSpan.MaxValue;
            this.introductory_price = null;
            this.introductory_price_cycles = 0;
            
            computableSubscriptionInfo = new AmazonComputableSubscriptionInfo(payload);
        }


        public static SubscriptionInfo CreateHuaweiSubscriptionInfo(string payload, string productId)
        {
            return new SubscriptionInfo()
            {
                productId = productId,
                computableSubscriptionInfo = new HuaweiComputableSubscriptionInfo(payload)
            };
        }
        
        #if UNITY_EDITOR
        
            public SubscriptionInfo(string productId) {
                this.productId = productId;
                computableSubscriptionInfo = new FakeComputableSubscriptionInfo(productId);
            }
            
        #endif
        
        public string getProductId() { return this.productId; }
        public DateTime getPurchaseDate() { return computableSubscriptionInfo.getPurchaseDate(); }
        public Result isSubscribed() { return computableSubscriptionInfo.isSubscribed(); }
        public Result isExpired() { return computableSubscriptionInfo.isExpired(); }
        public Result isCancelled() { return computableSubscriptionInfo.isCancelled(); }
        public Result isFreeTrial() { return computableSubscriptionInfo.isFreeTrial(); }
        public Result isAutoRenewing() { return computableSubscriptionInfo.isAutoRenewing(); }
        public TimeSpan getRemainingTime() { return computableSubscriptionInfo.getRemainingTime(); }
        public Result isIntroductoryPricePeriod() { return computableSubscriptionInfo.isIntroductoryPricePeriod(); }
        public TimeSpan getIntroductoryPricePeriod() { return this.introductory_price_period; }
        public string getIntroductoryPrice() { return string.IsNullOrEmpty(this.introductory_price) ? "not available" : this.introductory_price; }
        public long getIntroductoryPricePeriodCycles() { return this.introductory_price_cycles; }
        
        // these two dates are only for test Apple Store
        public DateTime getExpireDate() { return computableSubscriptionInfo.getExpireDate(); }
        public DateTime getCancelDate() { return this.subscriptionCancelDate; }
    
        // these two are for test Google Play store
        public TimeSpan getFreeTrialPeriod() { return this.freeTrialPeriod; }
        public TimeSpan getSubscriptionPeriod() { return this.subscriptionPeriod; }
        public string getFreeTrialPeriodString() { return this.free_trial_period_string; }
        public string getSkuDetails() { return this.sku_details; }
        public string getSubscriptionInfoJsonString() {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("productId", getProductId());
            dict.Add("is_free_trial", isFreeTrial());
            dict.Add("is_introductory_price_period", isIntroductoryPricePeriod() == Result.True);
            dict.Add("remaining_time_in_seconds", getRemainingTime().TotalSeconds);
            return MiniJson.JsonEncode(dict);
        }
    
        private DateTime nextBillingDate(DateTime billing_begin_date, TimeSpanUnits units) {

            if (units.days == 0.0 && units.months == 0 && units.years == 0) return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime next_billing_date = billing_begin_date;
            // find the next billing date that after the current date
            while (DateTime.Compare(next_billing_date, VerifiedDateTime) <= 0) {

                next_billing_date = next_billing_date.AddDays(units.days).AddMonths(units.months).AddYears(units.years);
            }
            return next_billing_date;
        }

        private TimeSpan accumulateIntroductoryDuration(TimeSpanUnits units, long cycles) {
            TimeSpan result = TimeSpan.Zero;
            for (long i = 0; i < cycles; i++) {
                result = result.Add(computePeriodTimeSpan(units));
            }
            return result;
        }
    
        private TimeSpan computePeriodTimeSpan(TimeSpanUnits units) {
            DateTime now = DateTime.Now;
            return now.AddDays(units.days).AddMonths(units.months).AddYears(units.years).Subtract(now);
        }

        private double computeExtraTime(string metadata, double new_sku_period_in_seconds) {
            var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(metadata);
            long old_sku_remaining_seconds = (long)wrapper["old_sku_remaining_seconds"];
            long old_sku_price_in_micros = (long)wrapper["old_sku_price_in_micros"];

            double old_sku_period_in_seconds = (parseTimeSpan((string)wrapper["old_sku_period_string"])).TotalSeconds;
            long new_sku_price_in_micros = (long)wrapper["new_sku_price_in_micros"];
            double result = ((((double)old_sku_remaining_seconds / (double)old_sku_period_in_seconds ) * (double)old_sku_price_in_micros) / (double)new_sku_price_in_micros) * new_sku_period_in_seconds;
            return result;
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
    }
}
