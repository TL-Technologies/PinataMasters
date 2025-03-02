using Modules.General;
using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    public class AdvertisingEditorRewardedVideoModuleImplementor : RewardedVideoModuleImplementor, IAdsInitializer
    {
        #region Fields

        AdvertisingEditorSettings advertisingEditorSettings;

        #endregion



        #region Class lifecycle

        public AdvertisingEditorRewardedVideoModuleImplementor(IAdvertisingService service, AdvertisingEditorSettings settings) : base(service)
        {
            advertisingEditorSettings = settings;
            AdvertisingEditorCanvas.OnEditorAdHide += AdvertisingEditorCanvas_OnEditorAdHide;
        }

        #endregion



        #region Properties

        public override bool IsVideoAvailable { get; protected set; }

        #endregion



        #region Methods

        public override void ShowVideo(string placementName)
        {
            AdvertisingEditorCanvas.Prefab.Instance.ShowRewardedVideo(placementName,
                advertisingEditorSettings.EditorVideoSettings,
                advertisingEditorSettings.EditorRewardedVideoModuleSettings,
                Invoke_OnAdShow);
        }


        public override void Invoke_OnAdShow(AdActionResultType responseResultType, int delay, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsVideoAvailable = false;
            }

            base.Invoke_OnAdShow(responseResultType, delay, errorDescription, adIdentifier);
        }


        public override void Invoke_OnAdRespond(int delay, AdActionResultType responseResultType, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsVideoAvailable = true;
            }

            base.Invoke_OnAdRespond(delay, responseResultType, errorDescription, adIdentifier);
        }


        public void Initialize()
        {
            DelayedAdRespondCall(advertisingEditorSettings.EditorRewardedVideoModuleSettings.FirstLoadDelay);
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
                DelayedAdRespondCall(advertisingEditorSettings.EditorRewardedVideoModuleSettings.ReloadDelay);
            }
        }

        #endregion
    }
}