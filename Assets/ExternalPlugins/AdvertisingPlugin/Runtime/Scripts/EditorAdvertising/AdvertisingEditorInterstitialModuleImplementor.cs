using Modules.General;
using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    public class AdvertisingEditorInterstitialModuleImplementor : InterstitialModuleImplementor, IAdsInitializer
    {
        #region Fields

        AdvertisingEditorSettings advertisingEditorSettings;

        #endregion



        #region Class lifecycle

        public AdvertisingEditorInterstitialModuleImplementor(IAdvertisingService service, AdvertisingEditorSettings settings) : base(service)
        {
            advertisingEditorSettings = settings;
            AdvertisingEditorCanvas.OnEditorAdHide += AdvertisingEditorCanvas_OnEditorAdHide;
        }

        #endregion



        #region Properties

        public override bool IsInterstitialAvailable { get; protected set; }

        #endregion



        #region Methods

        public override void ShowInterstitial(string placementName)
        {
            AdvertisingEditorCanvas.Prefab.Instance.ShowInterstitial(placementName,
                advertisingEditorSettings.EditorVideoSettings,
                advertisingEditorSettings.EditorInterstitialModuleSettings,
                Invoke_OnAdShow);
        }


        public override void Invoke_OnAdShow(AdActionResultType responseResultType, int delay, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsInterstitialAvailable = false;
            }
            
            base.Invoke_OnAdShow(responseResultType, delay, errorDescription, adIdentifier);
        }


        public override void Invoke_OnAdRespond(int delay, AdActionResultType responseResultType, string errorDescription, string adIdentifier)
        {
            if (responseResultType == AdActionResultType.Success)
            {
                IsInterstitialAvailable = true;
            }
            
            base.Invoke_OnAdRespond(delay, responseResultType, errorDescription, adIdentifier);
        }


        public void Initialize()
        {
            DelayedAdRespondCall(advertisingEditorSettings.EditorInterstitialModuleSettings.FirstLoadDelay);
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
                DelayedAdRespondCall(advertisingEditorSettings.EditorInterstitialModuleSettings.ReloadDelay);
            }
        }

        #endregion
    }
}