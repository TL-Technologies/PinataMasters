using Modules.General;
using System;
using System.Reflection;
using UnityEngine;


namespace Modules.Max
{
    public static class LLMaxManager
    {
        #region Fields

        private static Action<bool> OnCompleteCallback;
        
        #endregion



        #region Properties

        private static MaxPrivacyManager PrivacyManager { get; set; }

        #endregion

        

        #region Methods

        public static void Initialize(MaxPrivacyManager privacyManager, Action<bool> onCompleteCallback)
        {
            if (!LLMaxSettings.DoesInstanceExist)
            {
                Debug.LogError("[MaxAdvertisingServiceImplementor - Initialize] Need LLMaxSettings asset to init Max");
                onCompleteCallback(false);
                return;
            }

            if (string.IsNullOrEmpty(LLMaxSettings.Instance.SdkKey))
            {
                Debug.LogError("[MaxAdvertisingServiceImplementor - Initialize] no SdkKey specified in LLMaxSettings");
                onCompleteCallback(false);
                return;
            }

            PrivacyManager = privacyManager;
            OnCompleteCallback = onCompleteCallback;

            MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxSdkInitialized;
            MaxSdk.SetVerboseLogging(Debug.isDebugBuild);
            MaxSdk.SetSdkKey(LLMaxSettings.Instance.SdkKey);
            MaxSdk.InitializeSdk();
        }


        public static void ShowMediationDebugger()
        {
            MaxSdk.ShowMediationDebugger();
        }
        
        
        private static void ContinueInitialization()
        {
            MaxSdk.SetHasUserConsent(true);
            OnCompleteCallback(true);
        }

        #endregion



        #region Event handlers
        
        private static void OnMaxSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance, () => {
                MaxSdkCallbacks.OnSdkInitializedEvent -= OnMaxSdkInitialized;

                bool isPrivacyAvailable = sdkConfiguration.ConsentDialogState != MaxSdkBase.ConsentDialogState.DoesNotApply;
                
                #if UNITY_EDITOR || UNITY_IOS

                if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") !=
                    MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    PrivacyManager.WasPrivacyPopupsShown = true;
                    bool isAteEnabled = sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized;

                    var adSettingsClassType = Type.GetType("AudienceNetwork.AdSettings");
                    if (adSettingsClassType != null)
                    {
                        var method = adSettingsClassType.GetMethod("SetAdvertiserTrackingEnabled", BindingFlags.Static);
                        method.Invoke(null, new object[] {isAteEnabled});
                    }
                    ContinueInitialization();
                }
                else 
                    
                #endif
                
                if (isPrivacyAvailable && !PrivacyManager.WasPrivacyPopupsShown)
                {
                    PrivacyManager.ShowGdpr(ContinueInitialization);
                }
                else
                {
                    ContinueInitialization();
                }
                
            }, 0f);
        }

        #endregion
    }
}
