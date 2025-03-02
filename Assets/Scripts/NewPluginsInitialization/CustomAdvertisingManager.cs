using Modules.Advertising;
using Modules.General.Abstraction;
using PinataMasters;
using System;


public class CustomAdvertisingManager : Modules.Advertising.AdvertisingManager
{
    #region Fields

    public static event Action<AdModule> OnFullScreenAdStarted;
    public static event Action<AdModule> OnFullScreenAdFinished;
    public static event Action<bool> OnBannerVisibilityChanged;

    #endregion



    #region Methods

    public void ShowVideo(Action<bool> callback = null, string placement = AdPlacementType.DefaultPlacement)
    {
        TryShowAdByModule(AdModule.RewardedVideo, placement, AdShowCallBack);

        void AdShowCallBack(AdActionResultType adActionResultType)
        {
            if (adActionResultType == AdActionResultType.NoInternet)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);

                callback?.Invoke(false);
            }
            else
            {
                if (adActionResultType == AdActionResultType.NotAvailable)
                {
                    UIInfo.Prefab.Instance.Show(UIInfo.Type.NoAds);

                    callback?.Invoke(false);
                }
                else
                {
                    callback?.Invoke(adActionResultType == AdActionResultType.Success);
                }
            }
        }
    }


    public void ShowInterstitial(string placement, Action callback = null)
    {
        TryShowAdByModule(AdModule.Interstitial, placement, AdShowCallBack);

        void AdShowCallBack(AdActionResultType adActionResultType)
        {
            callback?.Invoke();
        }
    }
    

    public void ShowBanner()
    {
        UnlockAd(AdModule.Banner, typeof(CustomAdvertisingManager).ToString());
    }


    public void HideBanner()
    {
        LockAd(AdModule.Banner, typeof(CustomAdvertisingManager).ToString());
    }
    
    #endregion



    #region Advertising Manager ovveride
    
    protected override void FillPreDefinedAvailabilityParameters()
    {
        base.FillPreDefinedAvailabilityParameters();
        
        availabilityParametersLib.Add(new AdAvailabilityParameter(
            AdModule.Interstitial, 
            AdAvailabilityParameterType.NoShowingSubscription, 
            (placement) => !UISubscriptionSmart.IsShowing && !UINoSubscription.IsShowing && Story.WasStoryShowed,
            int.MaxValue));

        availabilityParametersLib.Add(new AdAvailabilityParameter(
            AdModule.Banner,
            AdAvailabilityParameterType.DefaultParameter,
            (placement) => !UISubscriptionSmart.IsShowing && !UINoSubscription.IsShowing && Story.WasStoryShowed && IsBannerAvailable(placement),
            int.MaxValue));
    }


    protected override void Controller_OnAdShow(AdModule adModule, AdActionResultType responseResultType, int delay,
        string errorDescription,
        string adIdentifier, string advertisingPlacement)
    {
        base.Controller_OnAdShow(adModule, responseResultType, delay, errorDescription, adIdentifier,
            advertisingPlacement);

        if (responseResultType == AdActionResultType.Success)
        {
            switch (adModule)
            {
                case AdModule.Interstitial:
                case AdModule.RewardedVideo:
                    OnFullScreenAdStarted?.Invoke(adModule);
                    break;

                case AdModule.Banner:
                    OnBannerVisibilityChanged?.Invoke(true);
                    break;
            }
        }
    }


    protected override void Controller_OnAdHide(AdModule adModule, AdActionResultType responseResultType,
        string errorDescription,
        string adIdentifier)
    {
        base.Controller_OnAdHide(adModule, responseResultType, errorDescription, adIdentifier);

        switch (adModule)
        {
            case AdModule.Interstitial:
            case AdModule.RewardedVideo:
                OnFullScreenAdFinished?.Invoke(adModule);
                break;

            case AdModule.Banner:
                OnBannerVisibilityChanged?.Invoke(false);
                break;
        }
    }

    #endregion
}
