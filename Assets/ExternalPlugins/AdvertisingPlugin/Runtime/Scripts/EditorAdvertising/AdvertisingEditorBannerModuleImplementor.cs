using Modules.General;
using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    public class AdvertisingEditorBannerModuleImplementor : BannerModuleImplementor, IAdsInitializer
    {
        #region Fields

        AdvertisingEditorSettings advertisingEditorSettings;

        #endregion



        #region Class lifecycle

        public AdvertisingEditorBannerModuleImplementor(IAdvertisingService service, AdvertisingEditorSettings settings) : base(service)
        {
            advertisingEditorSettings = settings;
            AdvertisingEditorCanvas.OnEditorAdHide += AdvertisingEditorCanvas_OnEditorAdHide;
        }

        #endregion



        #region Properties

        public override bool IsBannerAvailable { get; protected set; }

        #endregion



        #region Methods

        public override void ShowBanner(string placementName)
        {
            AdvertisingEditorCanvas.Prefab.Instance.ShowBanner(placementName,
                advertisingEditorSettings.EditorBannerSettings,
                Invoke_OnAdShow);
        }


        public override void HideBanner()
        {
            AdvertisingEditorCanvas.Prefab.Instance.HideAdView(AdModule.Banner, AdActionResultType.Success);
        }


        public override void Invoke_OnAdShow(AdActionResultType responseResultType, int delay, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsBannerAvailable = false;
            }

            base.Invoke_OnAdShow(responseResultType, delay, errorDescription, adIdentifier);
        }


        public override void Invoke_OnAdRespond(int delay, AdActionResultType responseResultType, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsBannerAvailable = true;
            }

            base.Invoke_OnAdRespond(delay, responseResultType, errorDescription, adIdentifier);
        }


        public void Initialize()
        {
            DelayedAdRespondCall(advertisingEditorSettings.EditorBannerSettings.FirstLoadDelay);
        }


        private void DelayedAdRespondCall(int delay)
        {
            Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance, () =>
            {
                Invoke_OnAdRespond(delay, AdActionResultType.Success, String.Empty, String.Empty);
            }, delay);
        }

        #endregion



        #region Events handlers

        private void AdvertisingEditorCanvas_OnEditorAdHide(AdModule module, AdActionResultType resultStatus)
        {
            if (module == AdModule)
            {
                Invoke_OnAdHide(resultStatus, null, "adIdentifier");
                DelayedAdRespondCall(advertisingEditorSettings.EditorBannerSettings.ReloadDelay);
            }
        }

        #endregion
    }
}