using DG.Tweening;
using Modules.General;
using Modules.InAppPurchase;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UISubscriptionSmart : UISubscriptionPopup
    {
        #region Types

        private enum Type
        {
            Trial,
            Weekly,
            Monthly,
            Yearly
        }


        [Serializable]
        private class SubscrioptionButton
        {
            public Type Type = Type.Trial;
            public Button Button = null;
            public Text PriceLabel = null;
            public MultiImageButton ImageButton = null;
            public Text profitText = null;
        }

        #endregion


        #region Variables

        private const string DEFAULT_USD_PRICE_LABEL = "${0}";

        private const string TRIAL_PRICE = "3 days free, \nthen {0} per week.";
        private const string WEEK_PRICE = "{0} per week.";
        private const string MONTH_PRICE = "{0} per month.";
        private const string YEAR_PRICE = "{0} per year.";

        private const string DESCRIPTION = "These purchases are auto-renewable subscription. Payment will be charged to your {0} account at the confirmation of purchase or at the end of the trial period of purchase, if it’s offered. The subscription automatically renews unless auto-renew is turned off at least 24 hours before the end of the current period. Your account will be charged for renewal within 24 hours prior to the end of the current period. You can manage and turn off auto-renewal of the subscription by going to your account settings on the {1} after purchase. Any unused portion of a free trial period, if it’s offered, will be forfeited when the user purchases a subscription to that publication, where applicable.\n\n\n";

        private const string PRIVACY_POLICY = "https://aigames.ae/policy#h.hn0lb3lfd0ij";
        private const string TERMS_OF_USE = "https://aigames.ae/policy#h.v7mztoso1wgw";

        public static readonly ResourceGameObject<UISubscriptionSmart> Prefab = new ResourceGameObject<UISubscriptionSmart>("Game/GUI/DialogSubscriptionSmart");

        [SerializeField]
        private RectTransform body = null;


        [Header("Buttons")]
        [SerializeField]
        private Button buttonBuy = null;
        [SerializeField]
        private List<SubscrioptionButton> buttons = null;
        [SerializeField]
        private Button termsOfUseButton = null;
        [SerializeField]
        private Button privacyPolicyButton = null;

        private Type selected;
        private bool isPremiumMustShow;
        private Action<UnitResult> onHidedAction;

        private SubscrioptionButton trialButton;
        private SubscrioptionButton weekButton;

        #endregion



        #region Properties

        public static bool IsShowing { get; private set; }

        public static bool IsFoolishPopUpAvailabe
        {
            get
            {
                return !IsShowing && !IAPs.IsSubscriptionActive && !IAPs.IsNoSubscriptionActive && Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonBuy.onClick.AddListener(BuySubscription);
            privacyPolicyButton.onClick.AddListener(PrivacyPolicy);
            termsOfUseButton.onClick.AddListener(TermsOfUse);

            foreach (SubscrioptionButton button in buttons)
            {
                button.Button.onClick.AddListener(() => SelectSubscription(button.Type));
            }

            trialButton = buttons.Find((btn) => btn.Type == Type.Trial);
            weekButton = buttons.Find((btn) => btn.Type == Type.Weekly);
        }

        #endregion



        #region Public methods

        public void Show(string placement = SubscriptionPurchasePlacement.Default, Action<UnitResult> onHided = null, Action onShowed = null)
        {
            subscriptionPlacement = placement;
            Show(onHided, onShowed);
        }


        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            onHidedAction = onHided;

            IsShowing = true;
            isPremiumMustShow = false;

            GameAnalytics.ShowSubcriptionEvent(SubscriptionType.BUTTON);

            body.anchoredPosition = new Vector2(-2000f, 0f);
            body.DOAnchorPos(Vector2.zero, 0.13f).OnComplete(() => Showed());

            SetWeeklyPrice();
            SetMonthlyPrice();
            SetYearlyPrice();

            description.text = string.Format(DESCRIPTION, accountTitle, storeTitle);

            if (!IAPs.IsPriceActual(IAPs.Name.SubscriptionWeekly))
            {
                IAPs.RequestPrice(IAPs.Name.SubscriptionWeekly, SetWeeklyPrice);
            }

            if (!IAPs.IsPriceActual(IAPs.Name.SubscriptionMonthly))
            {
                IAPs.RequestPrice(IAPs.Name.SubscriptionMonthly, SetMonthlyPrice);
            }

            if (!IAPs.IsPriceActual(IAPs.Name.SubscriptionYearly))
            {
                IAPs.RequestPrice(IAPs.Name.SubscriptionYearly, SetYearlyPrice);
            }

            trialButton.Button.gameObject.SetActive(IAPs.IsTrialSubscriptionAvailable);
            weekButton.Button.gameObject.SetActive(!IAPs.IsTrialSubscriptionAvailable);

            if (IAPs.IsTrialSubscriptionAvailable)
            {
                SelectSubscription(Type.Trial);
            }
            else
            {
                SelectSubscription(Type.Weekly);
            }
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            IsShowing = false;

            if (isPremiumMustShow)
            {
                EventSystemController.EnableEventSystem();
                gameObject.SetActive(false);
                TapZone.Lock(false);
            }
            else
            {
                Hided();
            }

            IAPs.CancelPriceRequest(IAPs.Name.SubscriptionWeekly, SetWeeklyPrice);
            IAPs.CancelPriceRequest(IAPs.Name.SubscriptionMonthly, SetMonthlyPrice);
            IAPs.CancelPriceRequest(IAPs.Name.SubscriptionYearly, SetYearlyPrice);
        }

        #endregion



        #region Private methods

        private void SetWeeklyPrice()
        {
            string subscriptionWeekItemPrice = IAPs.GetPrice(IAPs.Name.SubscriptionWeekly);
            string price = (string.IsNullOrEmpty(subscriptionWeekItemPrice)) ? (string.Format(DEFAULT_USD_PRICE_LABEL, IAPs.GetUSDPrice(IAPs.Name.SubscriptionWeekly))) : (subscriptionWeekItemPrice);

            if (!IAPs.IsTrialSubscriptionAvailable)
            {
                weekButton.PriceLabel.text = string.Format(WEEK_PRICE, price);
            }
            else
            {
                trialButton.PriceLabel.text = string.Format(TRIAL_PRICE, price);
            }
        }


        private void SetMonthlyPrice()
        {
            string subscriptionMonthItemPrice = IAPs.GetPrice(IAPs.Name.SubscriptionMonthly);

            string price = (string.IsNullOrEmpty(subscriptionMonthItemPrice)) ? (string.Format(DEFAULT_USD_PRICE_LABEL, IAPs.GetUSDPrice(IAPs.Name.SubscriptionMonthly))) : (subscriptionMonthItemPrice);

            Text priceText = buttons.Find((btn) => btn.Type == Type.Monthly).PriceLabel;
            priceText.text = string.Format(MONTH_PRICE, price);
        }


        private void SetYearlyPrice()
        {
            string subscriptionYearItemPrice = IAPs.GetPrice(IAPs.Name.SubscriptionYearly);

            string price = (string.IsNullOrEmpty(subscriptionYearItemPrice)) ? (string.Format(DEFAULT_USD_PRICE_LABEL, IAPs.GetUSDPrice(IAPs.Name.SubscriptionYearly))) : (subscriptionYearItemPrice);

            Text priceText = buttons.Find((btn) => btn.Type == Type.Yearly).PriceLabel;
            priceText.text = string.Format(YEAR_PRICE, price);
        }


        private void BuySubscription()
        {
            GameAnalytics.TrySubcriptionEvent(SubscriptionType.BUTTON);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else
            {
                IAPs.Name type;

                switch(selected)
                { 
                    case Type.Monthly:
                        type = IAPs.Name.SubscriptionMonthly;
                        break;

                    case Type.Yearly:
                        type = IAPs.Name.SubscriptionYearly;
                        break;

                    default:
                        type = IAPs.Name.SubscriptionWeekly;
                        break;
                }


                if (IAPs.IsPriceActual(type))
                {
                    EventSystemController.DisableEventSystem();
                    UILoader.Prefab.Instance.Show();
                    IAPs.Buy(type, OnDone);
                }
            }
        }


        private void OnDone(bool isBought)
        {
            UILoader.Prefab.Instance.Hide();

            if (isBought)
            {
                DataStateService.Instance.Set("placement", subscriptionPlacement);
                GameAnalytics.DoneSubcriptionEvent(SubscriptionType.BUTTON);
                CloudProgress.IsBlockedBySubscription = false;
                Hide();
            }

            EventSystemController.EnableEventSystem();
        }


        private void SelectSubscription(Type type)
        {
            selected = type;
            
            foreach(SubscrioptionButton button in buttons)
            {
                button.ImageButton.Interactable(button.Type == type);
                button.profitText.gameObject.SetActive(button.Type == type);
            }
        }


        protected override void ClosePopUp()
        {
            isPremiumMustShow = true;
            Hide();

            GameAnalytics.SkipSubcriptionEvent(SubscriptionType.BUTTON);
            UINoSubscription.Prefab.Instance.Show(onHidedAction);
        }


        private void PrivacyPolicy()
        {
            Application.OpenURL(PRIVACY_POLICY);
        }


        private void TermsOfUse()
        {
            Application.OpenURL(TERMS_OF_USE);
        }

        #endregion
    }
}
