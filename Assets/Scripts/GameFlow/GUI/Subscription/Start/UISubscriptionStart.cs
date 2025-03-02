using DG.Tweening;
using Modules.General;
using Modules.General.HelperClasses;
using Modules.InAppPurchase;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UISubscriptionStart : UISubscriptionPopup
    {

        #region Types

        [Serializable]
        private struct Step
        {
            public RectTransform Body;
            public Button nextButton;
            public UIDemo Demo;
            public Animator Animator;
            public AnimationClip AnimationClip;
            public Image FadeButtonImage;
        }

        #endregion



        #region Variables

        const string DefaultAndroidPrice = "5.99";
        private const string DEFAULT_CURRENCY_PRICE_LABEL = 
        #if UNITY_ANDROID
            "€{0}";
        #else
            "${0}";
        #endif
        private const string WAS_SHOWED = "was_start_subscription_showed";

        private const string PRICE_LABEL = "Then {0} per week.";
        private const string SUBSCRIPTION_DISCRIPTION = "Weekly Premium automatically renews for {0} per week after the 3-day free trial. Payment will be charged to your {1} account at the end of the trial period of purchase. The subscription automatically renews unless auto-renew is turned off at least 24 hours before the end of the current period. Your account will be charged for renewal within 24 hours prior to the end of the current period. You can manage and turn off auto-renewal of the subscription by going to your account settings on the {2} after purchase. Any unused portion of a free trial period will be forfeited when the user purchases a subscription to that publication, where applicable. \n\n";

        private const string PRIVACY_POLICY = "https://aigames.ae/policy#h.hn0lb3lfd0ij";
        private const string TERMS_OF_USE = "https://aigames.ae/policy#h.v7mztoso1wgw";

        private string SUBSCRIPTION_PANEL_DESC;

        public static readonly ResourceGameObject<UISubscriptionStart> Prefab = new ResourceGameObject<UISubscriptionStart>("Game/GUI/DialogSubscriptionStart");

        [SerializeField]
        private Button startButton = null;
        [SerializeField]
        private Button termsOfUseButton = null;
        [SerializeField]
        private Button privacyPolicyButton = null;
        [SerializeField]
        private Text priceLabel = null;

        [SerializeField]
        private RectTransform dotsPanel = null;
        [SerializeField]
        private MultiImageButton[] dots = null;

        [SerializeField]
        private Image fadeImage = null;

        [SerializeField]
        private Step[] steps = null;

        [SerializeField] UILoaderElement loaderPrefab;
        [SerializeField] string pricePlaceholderForLoader = " \t     ";
        
        UILoaderElement priceLoader;

        private int currentStep = 0;

        #endregion



        #region Properties

        public static bool IsShowing { get; private set; }

        public static bool WasShowed
        {
            get { return CustomPlayerPrefs.HasKey(WAS_SHOWED); }
            set { CustomPlayerPrefs.SetBool(WAS_SHOWED, value); }
        }


        public UILoaderElement PriceLoader
        {
            get
            {
                if (priceLoader == null)
                {
                    priceLoader = Instantiate(loaderPrefab, priceLabel.transform);
                }

                return priceLoader;
            }
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            GetComponent<Canvas>().worldCamera = Camera.main;

            startButton.onClick.AddListener(BuyStartSubscription);

            privacyPolicyButton.onClick.AddListener(PrivacyPolicy);
            termsOfUseButton.onClick.AddListener(TermsOfUse);

            steps[currentStep].nextButton.onClick.AddListener(NextStep);

            steps[currentStep].Body.gameObject.SetActive(true);
            steps[currentStep].nextButton.interactable = false;
            StartCoroutine(ActivateNextButton());

            dots[currentStep].Interactable(true);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            IsShowing = true;

            SetPrice();

            if (!IAPs.IsPriceActual(IAPs.Name.SubscriptionWeekly))
            {
                IAPs.RequestPrice(IAPs.Name.SubscriptionWeekly, SetPrice);
            }

            GameAnalytics.ShowSubcriptionEvent(SubscriptionType.START);

            steps[currentStep].AnimationClip.wrapMode = WrapMode.Once;
            steps[currentStep].Animator.Play(steps[currentStep].AnimationClip.name, 0);

            AudioManager.Instance.SetSnapshotForDemo();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            IsShowing = false;
            IAPs.CancelPriceRequest(IAPs.Name.SubscriptionWeekly, SetPrice);
            if (!WasShowed)
            {
                WasShowed = true;
            }

            AudioManager.Instance.NormalizeAudio();

            Hided();
        }

        #endregion



        #region Private methods

        private void SetPrice()
        {
            string subscriptionWeekItemPrice = IAPs.GetPrice(IAPs.Name.SubscriptionWeekly);
            bool isCorrectPrice = !string.IsNullOrEmpty(subscriptionWeekItemPrice);
            string defaultPrice = string.Format(DEFAULT_CURRENCY_PRICE_LABEL,
                #if UNITY_ANDROID
                    DefaultAndroidPrice
                #else
                    IAPs.GetUSDPrice(IAPs.Name.SubscriptionWeekly)
                #endif
                );
            
            PriceLoader.gameObject.SetActive(!isCorrectPrice);
            if (!isCorrectPrice)
            {
                priceLabel.text = string.Format(PRICE_LABEL, pricePlaceholderForLoader);
                PriceLoader.ShowOnParent(priceLabel.transform, GetSymbolPositionInText('\t') + Vector3.left * 50.0f, Vector3.one * 0.5f);
                description.text = string.Format(SUBSCRIPTION_DISCRIPTION, defaultPrice, accountTitle, storeTitle);
            }
            else
            {
                description.text = string.Format(SUBSCRIPTION_DISCRIPTION, subscriptionWeekItemPrice, accountTitle, storeTitle);
                priceLabel.text = string.Format(PRICE_LABEL, subscriptionWeekItemPrice);
            }
        }


        Vector3 GetSymbolPositionInText(char symbol)
        {
            TextGenerator textGenerator = new TextGenerator(priceLabel.text.Length);
            RectTransform rectTransform = priceLabel.gameObject.transform as RectTransform;

            if (rectTransform != null)
            {
                textGenerator.Populate(priceLabel.text, priceLabel.GetGenerationSettings(rectTransform.rect.size));

                int charIndex = priceLabel.text.IndexOf(symbol);
                if (charIndex > 0)
                {
                    Vector3 resultPosition = Vector3.zero;
                    resultPosition += priceLabel.gameObject.transform.position;
                    return resultPosition / 2f;
                }
            }

            return Vector3.zero;
        }


        private void BuyStartSubscription()
        {
            GameAnalytics.TrySubcriptionEvent(SubscriptionType.START);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else if (IAPs.IsPriceActual(IAPs.Name.SubscriptionWeekly))
            {
                EventSystemController.DisableEventSystem();
                UILoader.Prefab.Instance.Show();
                IAPs.Buy(IAPs.Name.SubscriptionWeekly, OnDone);
            }
        }


        private void OnDone(bool isBought)
        {
            UILoader.Prefab.Instance.Hide();

            if (isBought)
            {
                DataStateService.Instance.Set("placement", SubscriptionPurchasePlacement.ApplicationStart);
                GameAnalytics.DoneSubcriptionEvent(SubscriptionType.START);
                Hide();
            }

            EventSystemController.EnableEventSystem();
        }


        private void PrivacyPolicy()
        {
            Application.OpenURL(PRIVACY_POLICY);
        }


        private void TermsOfUse()
        {
            Application.OpenURL(TERMS_OF_USE);
        }


        protected override void ClosePopUp()
        {
            base.ClosePopUp();
            GameAnalytics.SkipSubcriptionEvent(SubscriptionType.START);
        }


        private void NextStep()
        {
            if (steps[currentStep].Demo != null)
            {
                steps[currentStep].Demo.DestroyPinata();
            }

            Destroy(steps[currentStep].Animator);
            steps[currentStep].FadeButtonImage.DOFade(1f, 0.5f);

            steps[currentStep].nextButton.onClick.RemoveListener(NextStep);
            steps[currentStep].nextButton.interactable = false;

            StartCoroutine(Fade());
        }


        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(2.5f);

            fadeImage.DOFade(1f, 0.3f).OnComplete(() =>
            {
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
                Next();
            });
        }

        private void Next()
        {
            steps[currentStep].Body.gameObject.SetActive(false);
            dots[currentStep].Interactable(false);

            currentStep++;
            steps[currentStep].Body.gameObject.SetActive(true);
            steps[currentStep].nextButton.interactable = false;
            StartCoroutine(ActivateNextButton());

            if (currentStep < steps.Length - 1)
            {
                steps[currentStep].nextButton.onClick.AddListener(NextStep);

                steps[currentStep].Animator.Play(steps[currentStep].AnimationClip.name, 0);
            }
            else
            {
                dotsPanel.gameObject.SetActive(false);

                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
                fadeImage.DOFade(0f, 1.3f).OnComplete(() => fadeImage.gameObject.SetActive(false));
            }

            dots[currentStep].Interactable(true);
        }


        private IEnumerator ActivateNextButton()
        {
            yield return new WaitForSeconds(2f);

            steps[currentStep].nextButton.interactable = true;
        }

        #endregion
    }
}