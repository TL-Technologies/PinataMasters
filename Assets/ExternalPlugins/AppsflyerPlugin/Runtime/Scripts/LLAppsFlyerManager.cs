using AOT;
using Http;
using Modules.General.HelperClasses;
using Modules.Hive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.AppsFlyer
{
    public sealed class LLAppsFlyerManager
    {
        #region Helpers

        public enum InitializationStatus
        {
            None = 0,
            Success = 1,
            Failed = 2,
            Warning = 3
        }

        #endregion



        #region Fields

        public static event Action<InitializationStatus> OnAppsFlyerInit;
        public static event Action OnConversionDataReceived;

        #if UNITY_IOS && !UNITY_EDITOR
            [DllImport ("__Internal")]
            static extern void LLAppsFlyerSetUserConsent(bool isConsentAvailable);

            [DllImport ("__Internal")]
            static extern void LLAppsFlyerInit(string appID, string devKey, string deviceId, bool isDebug, Action<int, string> onComplete, Action<string> onGetConversionData);

            [DllImport ("__Internal")]
            static extern void LLAppsFlyerLogEvent(string eventName, int numParams, string[] paramKeys, string[] paramVals);

            [DllImport ("__Internal")]
            static extern void LLAppsFlyerLogPurchase(string productName, string currencyCode, string price, string transactionId);

            [DllImport ("__Internal")]
            static extern string LLAppsFlyerUID();

            [DllImport ("__Internal")]
            static extern string LLAppsFlyerCampaignName();

            [DllImport ("__Internal")]
            static extern void LLAppsFlyerTrackCrossPromoteImpression(string appAppleId, string campaignName);

            [DllImport("__Internal")]
            static extern void LLAppsFlyerTrackAndOpenStore(string appAppleId, string campaignName, int numParams, string[] paramKeys, string[] paramValues);
        
            [DllImport ("__Internal")]
            static extern void LLAppsFlyerSetCustomerUserId(string customerUserId);

        #elif UNITY_ANDROID
            private const string METHOD_INIT = "LLAppsFlyerInit";
            private const string METHOD_SET_CONSENT = "LLAppsFlyerSetUserConsent";
            private const string METHOD_LOG_RICH_EVENT = "LLAppsFlyerLogRichEvent";
            private const string METHOD_LOG_PURCHASE = "LLAppsFlyerLogPurchase";
            private const string METHOD_GET_USER_ID = "LLAppsFlyerGetUserID";
            private const string METHOD_TRACK_CROSS_PROMO_CLICK = "LLAppsFlyerTrackCrossPromoClick";
            private const string METHOD_TRACK_CROSS_PROMO_SHOW = "LLAppsFlyerTrackCrossPromoShow";
            private const string METHOD_GET_CAMPAIGN_NAME = "LLAppsFlyerGetCampaignName";
            private const string METHOD_SET_CUSTOMER_USER_ID = "LLAppsFlyerSetCustomerUserId";
        #endif

        private const string ImpressionEndpoint = "https://impression.appsflyer.com";
        private const string ClickEndpoint = "https://app.appsflyer.com";
        private const string DefaultCampaignName = "CommonxPromo";

        private static bool IsInitializationStarted = false;
        private static bool isConsentSpecified = false;
        private static string campaignName = string.Empty;

        #endregion



        #region Public methods

        public static void SetUserConsent(bool isConsentAvailable)
        {
            CustomDebug.Log($"[AppsFlyer] Requested set user consent to {isConsentAvailable}");
            
            // according our terms of use and privacy policy 
            // user is not allowed to opt out from analytics
            #if UNITY_IOS && !UNITY_EDITOR
                LLAppsFlyerSetUserConsent(true);
            #endif

            #if UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic(METHOD_SET_CONSENT, true);
            #endif

            isConsentSpecified = true;
            
            CustomDebug.Log($"[AppsFlyer] Set user consent to {true}");
        }


        public static void Initialize(AppsFlyerSettings appsFlyerSettings, string deviceId)
        {
            if (IsInitializationStarted)
            {
                return;
            }
            IsInitializationStarted = true;

            if (!isConsentSpecified)
            {
                Debug.LogError("You should call SetUserConsent method before Initialize!");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
            else
            {
                #if UNITY_EDITOR
                    OnTrackingStarted((int) InitializationStatus.Success, string.Empty);
                #elif UNITY_IOS
                    LLAppsFlyerInit(
                        appsFlyerSettings.ApplicationIdentifier,
                        appsFlyerSettings.DeveloperKey,
                        deviceId,
                        appsFlyerSettings.IsDebugEnabled,
                        OnTrackingStarted,
                        OnGetConversionData);
                #elif UNITY_ANDROID
                    LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic(METHOD_INIT, 
                        appsFlyerSettings.DeveloperKey,
                        deviceId,
                        appsFlyerSettings.IsDebugEnabled,
                        LLAndroidJavaCallback.ProxyCallback(OnTrackingStarted),
                        LLAndroidJavaCallback.ProxyCallback(OnGetConversionData));
                #endif
            }
        }


        public static void Initialize(bool? isDebug = null)
        {
            DeviceInfo.RequestDeviceId(deviceId =>
            {
                Initialize(LLAppsFlyerSettings.GetAppsFlyerSettings(isDebug), deviceId);
            });
        }


        public static void LogEvent(string eventName, string value = null)
        {
            Dictionary<string, string> eventParams = new Dictionary<string, string>
            {
                {LLAppsFlyerEvents.PARAM_1, value ?? string.Empty}
            };

            LogRichEvent(eventName, eventParams);
        }


        //Purchase dictionary example
        //Dictionary<string, string> purchaseParams = new Dictionary<string, string>();       
        //purchaseParams.Add(LLAppsFlyerEvents.CURRENCY, "USD");
        //purchaseParams.Add(LLAppsFlyerEvents.REVENUE, "0.99");
        //purchaseParams.Add(LLAppsFlyerEvents.QUANTITY, "1");
        //purchaseParams.Add(LLAppsFlyerEvents.CONTENT_ID, "com.some.id");
        public static void LogRichEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            int numParameters = 0;
            string[] paramKeys = null;
            string[] paramValues = null;
            if (parameters != null)
            {
                numParameters = parameters.Count;
                paramKeys = new string[numParameters];
                parameters.Keys.CopyTo(paramKeys, 0);
                paramValues = new string[numParameters];
                parameters.Values.CopyTo(paramValues, 0);
            }

            #if UNITY_IOS && !UNITY_EDITOR
                LLAppsFlyerLogEvent(eventName, numParameters, paramKeys, paramValues);
            #elif UNITY_ANDROID && !UNITY_EDITOR
		        LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic(METHOD_LOG_RICH_EVENT, eventName, numParameters, paramKeys, paramValues);
            #endif
        }


        public static void LogPurchase(
            string productName,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson = "",
            string androidPurchaseSignature = "",
            string androidPublicKey = "")
        {
            #if UNITY_EDITOR
                return;
            #elif UNITY_IOS
                LLAppsFlyerLogPurchase(productName, currencyCode, price, transactionId);
            #elif UNITY_ANDROID
                AndroidTarget target = PlatformInfo.AndroidTarget;
                if (target == AndroidTarget.GooglePlay)
                {
                    LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic(METHOD_LOG_PURCHASE, androidPublicKey, androidPurchaseSignature, androidPurchaseDataJson, currencyCode, price);
                }
                else if (target == AndroidTarget.Amazon || target == AndroidTarget.Huawei)
                {
                    // On the Amazon and Huawei platforms, AppsFlyer can't send purchase event because of a lack of validation support,
                    // so we should send event directly
                    LogRichEvent(LLAppsFlyerEvents.PURCHASE, new Dictionary<string, string>()
                    {
                        { LLAppsFlyerEvents.REVENUE, price },
                        { LLAppsFlyerEvents.CURRENCY, currencyCode }
                    });
                }
                else
                {
                    throw new NotImplementedException();
                }
            #endif
        }


        public static string LLAppsFlyerGetAppsFlyerUID()
        {
            string result = string.Empty;

            #if UNITY_EDITOR
                result = "1487320338";
            #elif UNITY_IOS
                result = LLAppsFlyerUID();
            #elif UNITY_ANDROID
                result = LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic<string>(METHOD_GET_USER_ID);
            #endif

            return result;
        }


        public static string GetCampaignName()
        {
            if (!string.IsNullOrEmpty(campaignName))
            {
                return campaignName;
            }

            #if UNITY_IOS && !UNITY_EDITOR
                campaignName = LLAppsFlyerCampaignName();
            #elif UNITY_ANDROID && !UNITY_EDITOR
                campaignName = LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic<string>(METHOD_GET_CAMPAIGN_NAME);
            #endif

            return campaignName;
        }
        
        
        public static void SetCustomerUserId(string customerUserId)
        {
            #if UNITY_IOS && !UNITY_EDITOR
                LLAppsFlyerSetCustomerUserId(customerUserId);
            #elif UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLAppsFlyerManager>.CallStatic(METHOD_SET_CUSTOMER_USER_ID, customerUserId);
            #endif
        }


        /// <summary>
        /// Sends event to track cross-promo click.
        /// https://support.appsflyer.com/hc/en-us/articles/115004481946-Cross-promotion-attribution
        /// </summary>
        /// <param name="appId">The app package id on android; iTunes app id on ios.</param>
        /// <param name="parameters">Custom parameters to send with the event</param>
        /// <param name="callback">Optional callback to track response status. TRUE if response succeeded, FALSE otherwise</param>

        public static void TrackCrossPromoClick(string appId, IReadOnlyDictionary<string, string> parameters,
            Action<bool> callback = null)
        {
            IHttpClient client = new UnityHttpClient(ClickEndpoint);
            var request = client.Get($"/{appId}");
            var tokenSource = new CancellationTokenSource();

            foreach (var parameter in parameters)
            {
                request.SetParameter(parameter.Key, parameter.Value);
            }

            request.SendAsync(tokenSource.Token).ContinueWith(task =>
            {
                bool success;

                if (task.IsCompleted)
                {
                    var response = task.Result;

                    success = response.ResponseCode == HttpResponseCode.Ok;
                }
                else
                {
                    success = false;
                }

                tokenSource.Dispose();
                callback?.Invoke(success);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        /// <summary>
        /// Sends event to track cross-promo impression.
        /// https://support.appsflyer.com/hc/en-us/articles/115004481946-Cross-promotion-attribution
        /// </summary>
        /// <param name="appId">The app package id on android; iTunes app id on ios.</param>
        /// <param name="parameters">Custom parameters to send with the event</param>
        /// <param name="callback">Optional callback to track response status. TRUE if response succeeded, FALSE otherwise</param>
        public static void TrackCrossPromoImpression(string appId, IReadOnlyDictionary<string, string> parameters,
            Action<bool> callback = null)
        {
            IHttpClient client = new UnityHttpClient(ImpressionEndpoint);
            var request = client.Get($"/{appId}");
            var tokenSource = new CancellationTokenSource();

            foreach (var parameter in parameters)
            {
                request.SetParameter(parameter.Key, parameter.Value);
            }

            request.SendAsync(tokenSource.Token).ContinueWith(task =>
            {
                bool success;

                if (task.IsCompleted)
                {
                    var response = task.Result;

                    success = response.ResponseCode == HttpResponseCode.Ok;
                }
                else
                {
                    success = false;
                }

                tokenSource.Dispose();
                callback?.Invoke(success);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion



        #region Delegates

        [MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnTrackingStarted(int result, string errorDescription)
        {
            InitializationStatus initializationStatus = (InitializationStatus) result;
            switch (initializationStatus)
            {
                case InitializationStatus.Warning:
                    CustomDebug.LogWarning($"AppsFlyer initialization warning {errorDescription}");
                    break;
                case InitializationStatus.Failed:
                    CustomDebug.LogError($"AppsFlyer initialization fail {errorDescription}");
                    break;
                case InitializationStatus.None:
                    CustomDebug.LogError(
                        $"AppsFlyer initialization status {initializationStatus}. Something went wrong.");
                    break;
                default:
                    break;
            }
            
            OnAppsFlyerInit?.Invoke(initializationStatus);
        }


        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void OnGetConversionData(string result)
        {
            campaignName = result;

            if (campaignName.IsNullOrEmpty())
            {
                campaignName = DefaultCampaignName;
            }

            OnConversionDataReceived?.Invoke();
        }

        #endregion
    }
}
