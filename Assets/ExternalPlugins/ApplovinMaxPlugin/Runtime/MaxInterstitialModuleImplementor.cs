using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using UnityEngine;


namespace Modules.Max
{
    public class MaxInterstitialModuleImplementor : InterstitialModuleImplementor
    {
        #region Fields

        private int retryAttempt;
        private DateTime requestDate = DateTime.Now;
        private DateTime responseDate = DateTime.Now;

        #endregion



        #region Properties

        public int ResponseDelay => (int) Math.Round((DateTime.Now - requestDate).TotalMilliseconds);


        public int ShowDelay => (int) Math.Round((DateTime.Now - responseDate).TotalMilliseconds);


        public string InterstitialId => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.InterstitialId : string.Empty;
        
        
        public override bool IsInterstitialAvailable { get; protected set; }

        #endregion



        #region Methods

        public MaxInterstitialModuleImplementor(IAdvertisingService service) : base(service)
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        }


        public void LoadInterstitial()
        {
            IsInterstitialAvailable = false;
            requestDate = DateTime.Now;
            
            Debug.Log($"[MaxInterstitialModuleImplementor - LoadInterstitial] {InterstitialId}");
            
            MaxSdk.LoadInterstitial(InterstitialId);

            Invoke_OnAdRequested(InterstitialId);
        }


        public override void ShowInterstitial(string placementName)
        {
            MaxSdk.ShowInterstitial(InterstitialId, placementName);
        }

        #endregion
        
        
        
        #region Event handlers
        
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            IsInterstitialAvailable = true;
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
            responseDate = DateTime.Now;

            // Reset retry attempt
            retryAttempt = 0;
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Success, string.Empty, adUnitId);
        }


        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            responseDate = DateTime.Now;

            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

            Scheduler.Instance.CallMethodWithDelay(this, LoadInterstitial, (float)retryDelay);
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Error, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }


        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdShow(AdActionResultType.Success, ShowDelay, string.Empty, adUnitId);
        }


        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();

            Invoke_OnAdShow(AdActionResultType.Error, ShowDelay, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }


        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdClick(adUnitId);
        }


        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();

            Invoke_OnAdHide(AdActionResultType.Skip, string.Empty, adUnitId);
        }

        
        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"---> Inter1 revenue paid {adInfo.Revenue}");
            
            Invoke_OnImpressionDataReceive(MaxUtils.GetRevenueInfo(AdModule, adInfo), adUnitId, (float)adInfo.Revenue);
        }
        
        #endregion
    }
}