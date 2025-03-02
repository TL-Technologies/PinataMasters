using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Modules.AppsFlyer
{
    public class AppsFlyerCrossPromoTracker : ICrossPromoTracker
    {
        #region Fields

        public event Action OnConversionDataReceived;

        private const string EndpointApp = "https://app.appsflyer.com";

        private const string AdIdentifierKey = 
        #if UNITY_IOS
            "idfa";
        #else
            "advertising_id";
        #endif
        
        private IAdvertisingIdentifier advertisingIdentifierService;

        #endregion



        #region Properties

        public string CampaignName => LLAppsFlyerManager.GetCampaignName();

        #endregion



        #region Methods

        public void Initialize()
        {
            LLAppsFlyerManager.Initialize();
            LLAppsFlyerManager.OnConversionDataReceived += LLAppsFlyerManager_OnConversionDataReceived;
            
            advertisingIdentifierService = Services.GetService<IAdvertisingIdentifier>();
        }


        public void TrackImpression(ICrossPromoAdData data, string placement, string version)
        {
            GetAdvertisingId(id => LLAppsFlyerManager.TrackCrossPromoImpression(data.StoreId, GetData(data, id, placement, version)));
        }


        public void TrackClickAndOpenStore(ICrossPromoAdData data, string placement, string version)
        {
            GetAdvertisingId(id =>
            {
                LLAppsFlyerManager.TrackCrossPromoClick(data.StoreId, GetData(data, id, placement, version));
                string url = 
                #if UNITY_IOS && !UNITY_EDITOR
                    $"itms-apps://itunes.apple.com/app/apple-store/id{data.StoreId}?mt=8";
                #elif UNITY_ANDROID && !UNITY_EDITOR
                    $"market://details?id={data.StoreId}";
                #else
                    BuildUrl(EndpointApp, data.StoreId, GetData(data, id, placement, version));
                #endif
                
                Application.OpenURL(url);
            });
        }


        public void TrackAdShow(string eventName, string status, ICrossPromoAdData data, string isWatched = null)
        {
            GetAdvertisingId(id =>
            {
                Dictionary<string, string> customParams = new Dictionary<string, string>
                {
                    { "pid", "af_cross_promotion" },
                    { "status", status },
                    { "ad", data.PromotionName },
                    { AdIdentifierKey, id},
                    { "appname", data.AppId }
                };

                if (!string.IsNullOrEmpty(isWatched))
                {
                    customParams["is_watched"] = isWatched;
                }

                LLAppsFlyerManager.LogRichEvent(eventName, customParams);
            });
        }


        private void GetAdvertisingId(Action<string> callback)
        {
            advertisingIdentifierService.GetAdvertisingIdentifier(callback);
        }


        private Dictionary<string, string> GetData(ICrossPromoAdData data,
            string advertisingId, string placement, string version)
        {
            Dictionary<string, string> result = new Dictionary<string, string>
            {
                { "pid", data.MediaSource },
                { "c", data.CampaignName },
                { "af_c_id", data.CampainId },
                { "af_ad", data.PromotionName },
                { "af_siteid", data.CurrentAppId },
                { "af_sub1", version },
                { "af_sub2", placement },
                { "af_sub5", data.CurrentAppId },
                { "af_click_lookback", data.ClickLookbackPeriod },
                { "af_viewthrough_lookback", data.ViewthroughLookbackPeriod },
                { AdIdentifierKey, advertisingId}
            };

            return result;
        }


        private string BuildUrl(string baseUrl, string storeId, IReadOnlyDictionary<string, string> parameters)
        {
            var url = new StringBuilder(baseUrl.Length * 2);
            url.Append(baseUrl);
            url.Append('/');
            url.Append(storeId);
            url.Append('?');
            foreach (var pair in parameters)
            {
                url.Append(pair.Key);
                url.Append('=');
                url.Append(pair.Value);
                url.Append('&');
            }
            url.Length = url.Length - 1;
            
            return url.ToString();
        }

        #endregion



        #region Event handlers

        private void LLAppsFlyerManager_OnConversionDataReceived()
        {
            LLAppsFlyerManager.OnConversionDataReceived -= LLAppsFlyerManager_OnConversionDataReceived;
            OnConversionDataReceived?.Invoke();
        }

        #endregion
    }
}
