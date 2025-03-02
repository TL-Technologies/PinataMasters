using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PinataMasters
{
    public class UISubscriptionPopup : UIUnit<UnitResult>
    {
        #region Variables

        [SerializeField]
        protected Button closeButton = null;
        [SerializeField]
        protected Text description = null;
        [SerializeField]
        private Button restoreButton = null;

#if UNITY_IOS
        protected const string accountTitle = "Apple ID";
        protected const string storeTitle = "App Store";
#elif UNITY_ANDROID
        protected const string accountTitle = "Google";
        protected const string storeTitle = "Google Play";
#endif

        protected string subscriptionPlacement = null;
        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

#if UNITY_IOS
            restoreButton.gameObject.SetActive(true);
#else
            restoreButton.gameObject.SetActive(false);
#endif

            restoreButton.onClick.AddListener(TryRestore);
            closeButton.onClick.AddListener(ClosePopUp);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);
            AdvertisingHelper.HideBanner();
            TapZone.Lock(true);
            Showed();
        }

        #endregion



        #region Private methods

        protected virtual void ClosePopUp()
        {
            Hide();
        }

        protected override void Hided(UnitResult result = null)
        {
            if (!IAPs.IsSubscriptionActive && !IAPs.IsNoSubscriptionActive)
            {
                AdvertisingHelper.ShowBanner();
            }

            TapZone.Lock(false);

            base.Hided(result);
        }


        private void TryRestore()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
                return;
            }

            GameAnalytics.SendTryRestoreEvent();
            EventSystemController.DisableEventSystem();
            UILoader.Prefab.Instance.Show();

            IAPs.RestorePurchase((bool success) =>
            {
                UILoader.Prefab.Instance.Hide();
                EventSystemController.EnableEventSystem();
                if (success)
                {
                    UIInfo.Prefab.Instance.Show(UIInfo.Type.Restore, (_) => OnInfoClose());
                    if (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive)
                    {
                        GameAnalytics.SendDoneRestoreEvent();
                    }
                }
            });
        }


        private void OnInfoClose()
        {
            if (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive)
            {
                Hide();
            }
        }

        #endregion
    }
}
