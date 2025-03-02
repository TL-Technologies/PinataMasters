using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class AmazonComputableSubscriptionInfo : IComputableSubscriptionInfo
    {
        private static readonly Dictionary<string, int> MonthNumberByName = new Dictionary<string, int>()
        {
            { "Jan", 1 },
            { "Feb", 2 },
            { "Mar", 3 },
            { "Apr", 4 },
            { "May", 5 },
            { "Jun", 6 },
            { "Jul", 7 },
            { "Aug", 8 },
            { "Sep", 9 },
            { "Oct", 10 },
            { "Nov", 11 },
            { "Dec", 12 },
        };
        private DateTime purchaseDate;
        
        
        public AmazonComputableSubscriptionInfo(string payload)
        {
            purchaseDate = GetPurchaseDateFromPayload(payload);
        }
        
        
        public DateTime getPurchaseDate() => purchaseDate;


        public Result isSubscribed() => Result.True;


        public Result isExpired() => Result.False;


        public Result isCancelled() => Result.Unsupported;
        
        
        public Result isFreeTrial() => Result.Unsupported;


        public Result isAutoRenewing() => Result.Unsupported;


        public TimeSpan getRemainingTime() => TimeSpan.MaxValue;


        public Result isIntroductoryPricePeriod() => Result.Unsupported;


        public DateTime getExpireDate() => DateTime.MaxValue;
        
        
        private DateTime GetPurchaseDateFromPayload(string payload)
        {
            // Payload example
            // {
            //     "receiptId":"DctNC4IwGADg__KebTTFDYVuSqGFhwi6vrSh4r6crhjRf2_35_lCgBpUcbnyp3PtbfLDuTHBV4eg3qvV_j6yNQ5Nr3HiTHWPE2SwpfKymjiFcZRGoI9kR7NsZEZHxIzaGvGRclGRJr5DnWcgUqKM8uqYl4zxsoDfHw",
            //     "userId":"l3HL7XppEMhrOGDnur9-ulvqomrSg6qyODKmah76lJU=",
            //     "isSandbox":true,
            //     "productJson":
            //     {
            //         "sku":"com.playgendary.tanks.iap.diamondweekly1",
            //         "productType":"SUBSCRIPTION",
            //         "description":"Buy Diamond Membership",
            //         "price":"$7.99",
            //         "smallIconUrl":"http:\/\/",
            //         "title":"Diamond Membership",
            //         "coinsRewardAmount":0
            //     },
            //     "receiptJson":
            //     {
            //         "receiptId":"DctNC4IwGADg__KebTTFDYVuSqGFhwi6vrSh4r6crhjRf2_35_lCgBpUcbnyp3PtbfLDuTHBV4eg3qvV_j6yNQ5Nr3HiTHWPE2SwpfKymjiFcZRGoI9kR7NsZEZHxIzaGvGRclGRJr5DnWcgUqKM8uqYl4zxsoDfHw",
            //         "sku":"com.playgendary.tanks.iap.diamondweekly1",
            //         "itemType":"SUBSCRIPTION",
            //         "purchaseDate":"Thu Apr 08 20:22:46 GMT+03:00 2021"
            //     }
            // }
            
            Dictionary<string, object> payloadWrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            Dictionary<string, object> receiptJson = payloadWrapper["receiptJson"] as Dictionary<string, object>;
            if (receiptJson == null)
            {
                return DateTime.MinValue;
            }
            
            string purchaseDateString = receiptJson["purchaseDate"] as string;
            if (string.IsNullOrEmpty(purchaseDateString))
            {
                return DateTime.MinValue;
            }

            DateTime result = new DateTime();

            try
            {
                // Parse purchase date in format "Thu Apr 08 20:22:46 GMT+03:00 2021" or "Thu Apr 08 20:22:46 GMT 03:00 2021"
                string[] splitDate = purchaseDateString.Split(' ');
                
                bool isDateWithoutPlus = splitDate.Length == 7;

                int month = MonthNumberByName[splitDate[1]];
                int day = int.Parse(splitDate[2]);
                int year = int.Parse(splitDate[splitDate.Length - 1]);

                string[] splitTime = splitDate[3].Split(':');
                int hour = int.Parse(splitTime[0]);
                int minute = int.Parse(splitTime[1]);
                int second = int.Parse(splitTime[2]);

                bool isPositiveGmt;
                string[] splitGmt;
                if (isDateWithoutPlus)
                {
                    isPositiveGmt = true; 
                    splitGmt = splitDate[5].Split(':');
                }
                else
                {
                    isPositiveGmt = splitDate[4].Contains("+");
                    splitGmt = splitDate[4].Substring(4).Split(':');
                }
                int gmtHour = int.Parse(splitGmt[0]);
                int gmtMinute = int.Parse(splitGmt[1]);

                result = new DateTime(year, month, day, hour, minute, second);
                TimeSpan gmtShift = new TimeSpan(0, gmtHour, gmtMinute, 0);
                result = isPositiveGmt ? result - gmtShift : result + gmtShift;
            }
            catch (Exception e)
            {
                CustomDebug.LogError($"Storemanager: can't parse purchase date. Message: {e.Message}, StackTrace: {e.StackTrace}");
            }

            return result;
        }
    }
}
