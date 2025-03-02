using MiniJSON;
using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


namespace Modules.Max
{
    public static class MaxUtils
    {
        private static string appVersion;

        public static void Initialize()
        {
            appVersion = Application.version;
        }
        
        
        public static string GetRevenueInfo(AdModule adModule, MaxSdkBase.AdInfo adInfo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"ad_format", Enum.GetName(typeof(AdModule), adModule)},
                {"country_code", MaxSdk.GetSdkConfiguration().CountryCode},
                {"id", adInfo.CreativeIdentifier},
                {"max_ad_unit_id", adInfo.AdUnitIdentifier},
                {"network_name", adInfo.NetworkName},
                {"revenue", adInfo.Revenue.ToString(CultureInfo.InvariantCulture)},
                {"third_party_ad_placement_id", adInfo.Placement},
                {"app_version", appVersion},
                {"user_segment", MaxSdk.UserSegment.Name}
            };
            return Json.Serialize(data);
        }
    }
}