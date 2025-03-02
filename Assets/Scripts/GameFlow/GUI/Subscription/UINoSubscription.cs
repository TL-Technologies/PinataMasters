using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace PinataMasters
{
    public class UINoSubscription : UISubscriptionPopup
    {
        #region Variables

        private const string DEFAULT_USD_PRICE_LABEL = "${0}";

        public static readonly ResourceGameObject<UINoSubscription> Prefab = new ResourceGameObject<UINoSubscription>("Game/GUI/DialogNoSubscription");

        [SerializeField]
        private Button NoSubscriptionButton = null;
        [SerializeField]
        private Text priceLabel = null;
        [SerializeField]
        private RectTransform body = null;

        #endregion



        #region Properties

        public static bool IsShowing { get; private set; }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            GetComponent<Canvas>().worldCamera = Camera.main;

            NoSubscriptionButton.onClick.AddListener(BuyNoSubscription);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            IsShowing = true;

            SetPrice();

            if (!IAPs.IsPriceActual(IAPs.Name.NoSubscription))
            {
                IAPs.RequestPrice(IAPs.Name.NoSubscription, SetPrice);
            }

            body.anchoredPosition = new Vector2(-2000f, 0f);
            body.DOAnchorPos(Vector2.zero, 0.13f).OnComplete(() => Showed());
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            IsShowing = false;
            IAPs.CancelPriceRequest(IAPs.Name.NoSubscription, SetPrice);

            Hided();

            if (CloudProgress.IsBlockedBySubscription)
            {
                CloudProgress.IsBlockedBySubscription = false;
                CloudProgress.TryShowRestoreProgress();
            }
        }
        
        
        protected override void Hided(UnitResult result = null)
        {
            if (!IAPs.IsSubscriptionActive && !IAPs.IsNoSubscriptionActive)
            {
                AdvertisingHelper.ShowBanner();
            }

            base.Hided(result);
        }

        #endregion



        #region Private methods

        private void SetPrice()
        {
            string noSubscriptionItemPrice = IAPs.GetPrice(IAPs.Name.NoSubscription);

            string price = (string.IsNullOrEmpty(noSubscriptionItemPrice)) ? (string.Format(DEFAULT_USD_PRICE_LABEL, IAPs.GetUSDPrice(IAPs.Name.NoSubscription))) : (noSubscriptionItemPrice);

            priceLabel.text = price;
        }


        private void BuyNoSubscription()
        {
            GameAnalytics.TrySubcriptionEvent(SubscriptionType.NO_SUBSCRIPTION);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else
            {
                if (IAPs.IsPriceActual(IAPs.Name.NoSubscription))
                {
                    EventSystemController.DisableEventSystem();
                    UILoader.Prefab.Instance.Show();
                    IAPs.Buy(IAPs.Name.NoSubscription, OnDone);
                }
            }
        }


        private void OnDone(bool isBought)
        {
            UILoader.Prefab.Instance.Hide();

            if (isBought)
            {
                GameAnalytics.DoneSubcriptionEvent(SubscriptionType.NO_SUBSCRIPTION);
                Hide();
            }

            EventSystemController.EnableEventSystem();
        }


        protected override void ClosePopUp()
        {
            base.ClosePopUp();
            GameAnalytics.SkipSubcriptionEvent(SubscriptionType.NO_SUBSCRIPTION);
        }

        #endregion
    }
}
