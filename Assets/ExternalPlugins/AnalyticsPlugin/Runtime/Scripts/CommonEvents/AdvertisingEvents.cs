using System;
using MiniJSON;
using System.Collections.Generic;
using System.Globalization;


namespace Modules.Analytics
{
    public static partial class CommonEvents
    {
        #region Common ad events

        public static void SendAdRequest(
            string serviceName, 
            string adModule, 
            string adIdentifier)
        {
            // string eventName = string.Format("{0}_{1}_request", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            //     {
            //         { "unit_id", adIdentifier },
            //         { "session_id", sessionId },
            //     });
        }


        public static void SendAdRespond(
            string serviceName, 
            string adModule, 
            int delay,
            string responseResultType, 
            string errorDescription, 
            string adIdentifier)
        {
            // string eventName = string.Format("{0}_{1}_answer", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            // {
            //     {"delay", delay.ToString()},
            //     {"unit_id", adIdentifier},
            //     {"status", responseResultType},
            //     {"error_description", errorDescription},
            //     {"session_id", sessionId},
            // });
        }


        public static void SendImpressionData(
            string serviceName, 
            string impressionJsonData, 
            string adIdentifier, 
            float revenue)
        {
            if (revenue < 0.0f)
            {
                return;
            }
            
            Dictionary<string, string> data = Json.Deserialize<Dictionary<string, string>>(impressionJsonData);
            
            String networkName = "";
            data.TryGetValue("network_name", out networkName);
            String location = "";
            data.TryGetValue("country_code", out location);
        }


        public static void SendAdShow(
            string serviceName, 
            string adModule, 
            string responseResultType,
            int delay, 
            string errorDescription, 
            string adIdentifier, 
            string placement)
        {
            // string eventName = string.Format("{0}_{1}_show", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            // {
            //     {"delay", delay.ToString()},
            //     {"placement", placement},
            //     {"unit_id", adIdentifier},
            //     {"status", responseResultType},
            //     {"error_description", errorDescription},
            //     {"session_id", sessionId},
            // });
        }


        public static void SendAdClick(
            string serviceName, 
            string adModule, 
            string adIdentifier, 
            string placement)
        {
            // string eventName = string.Format("{0}_{1}_click", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            // {
            //     {"placement", placement},
            //     {"unit_id", adIdentifier},
            //     {"session_id", sessionId},
            // });
        }

        
        public static void SendAdExpire(
            string serviceName, 
            string adModule, 
            int delay, 
            string adIdentifier)
        {
            // string eventName = string.Format("{0}_{1}_expire", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            // {
            //     {"unit_id", adIdentifier},
            //     {"delay", delay.ToString()},
            //     {"session_id", sessionId},
            // });
        }


        public static void SendAdAvailabilityCheck(
            string serviceName, 
            string adModule,
            bool adAvailabilityStatus,
            string adPlacement,
            string errorDescription)
        {
            // string eventName = string.Format("{0}_{1}_availability_check", serviceName, adModule);
            // AnalyticsManager.Instance.SendEvent(eventName, new Dictionary<string, string>
            // {
            //     {"ad_availability_status", adAvailabilityStatus.ToString()},
            //     {"session_id", sessionId},
            //     {"placement", adPlacement},
            //     {"error_description", errorDescription},
            //     {"eventId", DateTime.Now.Millisecond.ToString()} //special param for AF (in order to separate events)
            // });
        }
        
        #endregion
    }
}