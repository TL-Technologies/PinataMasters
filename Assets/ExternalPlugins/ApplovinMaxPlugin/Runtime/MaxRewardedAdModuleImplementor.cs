using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using UnityEngine;


namespace Modules.Max
{
    public class MaxRewardedAdModuleImplementor : RewardedVideoModuleImplementor
    {
        #region Fields

        private int retryAttempt = 0;
        private DateTime requestDate = DateTime.Now;
        private DateTime responseDate = DateTime.Now;
        private bool isWatched;

        #endregion



        #region Properties

        public int ResponseDelay => (int) Math.Round((DateTime.Now - requestDate).TotalMilliseconds);


        public int ShowDelay => (int) Math.Round((DateTime.Now - responseDate).TotalMilliseconds);
        

        public string RewardedId => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.RewardedId : string.Empty;
        
        
        public override bool IsVideoAvailable { get; protected set; }

        #endregion



        #region Methods

        public MaxRewardedAdModuleImplementor(IAdvertisingService service) : base(service)
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }


        public override void ShowVideo(string placementName)
        {
            Debug.Log($"[MaxRewardedAdModuleImplementor - ShowVideo] {RewardedId}");
            isWatched = false;

            MaxSdk.ShowRewardedAd(RewardedId, placementName);
        }


        public void LoadRewardedAd()
        {
            IsVideoAvailable = false;
            requestDate = DateTime.Now;
            MaxSdk.LoadRewardedAd(RewardedId);
            Invoke_OnAdRequested(RewardedId);
        }

        #endregion



        #region Event handlers

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            IsVideoAvailable = true;
            responseDate = DateTime.Now;
            // Reset retry attempt
            retryAttempt = 0;
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Success, string.Empty, adUnitId);
        }
        
        
        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            responseDate = DateTime.Now;
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        
            retryAttempt++;
            float retryDelay = (float)Math.Pow(2, Math.Min(6, retryAttempt));

            Scheduler.Instance.CallMethodWithDelay(this, LoadRewardedAd, retryDelay);
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Error, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }

        
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdShow(AdActionResultType.Success, ShowDelay, string.Empty, adUnitId);
        }
        
        
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();

            Invoke_OnAdShow(AdActionResultType.Error, ShowDelay, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }

        
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdClick(adUnitId);
        }
        
        
        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();

            if (!isWatched)
            {
                Invoke_OnAdHide(AdActionResultType.Skip, string.Empty, adUnitId);
            }
        }
        
        
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            isWatched = true;

            Invoke_OnAdHide(AdActionResultType.Success, string.Empty, adUnitId);
        }
        
        
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            Debug.Log($"---> Rewarded1 revenue {adInfo.Revenue}");
            
            Invoke_OnImpressionDataReceive(MaxUtils.GetRevenueInfo(AdModule, adInfo), adUnitId, (float)adInfo.Revenue);
        } 

        #endregion
    }
}