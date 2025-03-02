using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIPlayerMenu : UITweensUnit<UnitResult>
    {
        #region Types

        [Serializable]
        private struct Tutorial
        {
            public Button Button;
            public RectTransform Hand;
            public MenuPlayerPanel Panel;
            public Image MaskImage;
        }

        #endregion



        #region Variables

        public static event Action OnPlayerMenuShow = delegate { };
        public static event Action OnSubscriptionClick = delegate { };
        public static event Action<float, string> OnMiniBankClick = delegate { };

        public static readonly ResourceGameObject<UIPlayerMenu> Prefab = new ResourceGameObject<UIPlayerMenu>("Game/GUI/MenuPlayer");

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Tabs topPanel = null;
        [SerializeField]
        private Tabs bottomPanel = null;
        [SerializeField]
        private RectTransform tapToStart = null;
        [SerializeField]
        private float tapToStartShowDelay = 3f;
        [SerializeField]
        private Button buttonClose = null;
        [SerializeField]
        private Button buttonDebugMenu = null;
        [SerializeField]
        private Button buttonSubscription = null;
        [SerializeField]
        private MultiImageButton multiImageButtonSubscription = null;
        [SerializeField]
        private ParticleSystem effectButtonSubscription = null;
        [SerializeField]
        private Text textCoins = null;
        [SerializeField]
        private TextCounterEffect textGems = null;
        [SerializeField]
        private Button buttonMiniBank = null;
        [SerializeField]
        private WeaponPanel weaponPanel = null;

        [Header("Effects")]
        [SerializeField]
        private ParticleSystem addGemsEffect = null;

        [Header("Tutorials")]
        [SerializeField]
        private Tutorial prestigeTutorial = new Tutorial();
        [SerializeField]
        private Tutorial coinsTutorial = new Tutorial();

        private RectTransform topPanelTransform;
        private RectTransform bottomPanelTransform;

        private Tutorial currentTutorial;

        public bool shouldShow = true;
        private IStoreManager storeManager = null;
        #endregion



        #region Properties

        private Vector2 SaveTopOffset { get; set; }

        private Vector2 SaveTopWithBannerOffset { get; set; }

        private IStoreManager StoreManager

        {
            get
            {
                if (storeManager == null)
                {
                    storeManager = Services.GetService<IStoreManager>();
                }

                return storeManager;
            }
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            topPanelTransform = topPanel.GetComponent<RectTransform>();
            bottomPanelTransform = bottomPanel.GetComponent<RectTransform>();

            Rect canvasRect = GetComponent<RectTransform>().rect;
            SaveTopOffset = new Vector2(topPanelTransform.anchorMax.x, topPanelTransform.anchorMax.y - SafeOffset.GetSafeTopRatio(canvasRect));
            SaveTopWithBannerOffset = new Vector2(topPanelTransform.anchorMax.x, topPanelTransform.anchorMax.y - SafeOffset.GetSafeTopWithBannerRatio(canvasRect));

            topPanelTransform.anchorMax = SaveTopOffset;
            bottomPanelTransform.anchorMin = new Vector2(bottomPanelTransform.anchorMin.x, bottomPanelTransform.anchorMin.y);

            buttonClose.onClick.AddListener(OnClickClose);
            buttonSubscription.onClick.AddListener(OnClickSubscription);
            buttonMiniBank.onClick.AddListener(ShowMiniBank);

            #if DEBUG_TARGET
            buttonDebugMenu.gameObject.SetActive(true);
            buttonDebugMenu.onClick.AddListener(() => UIDebugMenu.Prefab.Instance.Show());
            #endif
            RefreshCoinText();
            RefreshGemText();

            Player.OnChangeCoins += RefreshCoinText;
            Player.OnChangeGems += RefreshGemText;
            Player.OnIncreaseGems += PlayGemEffect;
            Player.OnPrefsUpdated += Refresh;
            CustomAdvertisingManager.OnBannerVisibilityChanged += CustomAdvertisingManagerOnBannerVisibilityChanged;
            TutorialManager.OnNeedTapToStart += OnNeedTapToStart;
            ViewSkin.OnSkinBought += ShowUnlockDialog;
            StoreManager.PurchaseComplete += StoreManager_PurchaseComplete;
        }


        private void OnDestroy()
        {
            Player.OnChangeCoins -= RefreshCoinText;
            Player.OnChangeGems -= RefreshGemText;
            Player.OnIncreaseGems -= PlayGemEffect;
            Player.OnPrefsUpdated -= Refresh;
            CustomAdvertisingManager.OnBannerVisibilityChanged -= CustomAdvertisingManagerOnBannerVisibilityChanged;
            TutorialManager.OnNeedTapToStart -= OnNeedTapToStart;
            ViewSkin.OnSkinBought -= ShowUnlockDialog;
            StoreManager.PurchaseComplete -= StoreManager_PurchaseComplete;
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            SetSubscriptionButtonState();

            bottomPanel.gameObject.SetActive(TutorialManager.Instance.IsUpgradeAbilityTutorialCanStart || TutorialManager.Instance.IsUpgradeAbilityTutorialPassed);

            base.Show(onHided, onShowed);

            tweenColor.Duration = durationShow;
            tweenColor.Play();

            tapToStart.gameObject.SetActive(false);

            CustomAdvertisingManagerOnBannerVisibilityChanged(
                (CustomAdvertisingManager.Instance as AdvertisingManager).IsBannerShowing);

            if (TutorialManager.Instance.IsUpgradeAbilityTutorialPassed)
            {
                bottomPanel.SetAlert();
            }

            if (TutorialManager.Instance.IsUpgradeWeaponTutorialPassed)
            {
                topPanel.SetAlert();
            }

            OnPlayerMenuShow();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tapToStart.gameObject.SetActive(false);

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }


        public void EnableOrDisableUI()
        {
            shouldShow = !shouldShow;
            topPanel.gameObject.SetActive(shouldShow);
            bottomPanel.gameObject.SetActive(shouldShow);
            tapToStart.gameObject.SetActive(shouldShow);
        }

        #endregion



        #region Private Methods

        protected override void Showed()
        {
            base.Showed();

            if (TutorialManager.Instance.IsUpgradeWeaponTutorialCanStart)
            {
                weaponPanel.SetUpgradeTutorialState();
            }
            else if (TutorialManager.Instance.IsBuyTutorialCanStart)
            {
                weaponPanel.SetBuyTutorialState();
            }
            else if (TutorialManager.Instance.IsUpgradeAbilityTutorialCanStart)
            {
                SetTutorialState(coinsTutorial);
            }
            else if (TutorialManager.Instance.IsPrestigeTutorialCanStart)
            {
                SetTutorialState(prestigeTutorial);
            }
            else
            {
                StartCoroutine(ShowTapToStart(tapToStartShowDelay));
            }
        }


        private void ShowUnlockDialog(int skin)
        {
            UICharacterUnlock.Prefab.Instance.Show(Player.CurrentSkin, (_) => tapToStart.gameObject.SetActive(true), () => tapToStart.gameObject.SetActive(false));
        }


        private void OnClickClose()
        {
            Hide();
        }


        private void OnClickSubscription()
        {
            if (!IAPs.IsSubscriptionActive && !IAPs.IsNoSubscriptionActive) OnSubscriptionClick();
        }


        private void RefreshCoinText()
        {
            textCoins.text = Player.Coins.ToShortFormat();
        }


        private void RefreshGemText()
        {
            textGems.Refresh(Player.Gems);
        }


        private void PlayGemEffect()
        {
            addGemsEffect.Play(true);
        }
        

        private void OnNeedTapToStart()
        {
            StartCoroutine(ShowTapToStart(0f));
        }


        private void ShowMiniBank()
        {
            int lastBoughtIndex = 0;
            for (int i = 0; i < Arsenal.Count; i++)
            {
                lastBoughtIndex = Player.IsWeaponBought(i) ? i : lastBoughtIndex;
            }

            OnMiniBankClick(Arsenal.GetWeaponPrice(lastBoughtIndex), MiniBankPlacement.BUTTON_MENU_BANK);
        }


        private IEnumerator ShowTapToStart(float delay)
        {
            yield return new WaitForSeconds(delay);

            tapToStart.gameObject.SetActive(true);

            float bottomPoint = bottomPanel.gameObject.activeInHierarchy ? bottomPanelTransform.offsetMax.y : 0f;
            float topPoint = topPanelTransform.anchorMin.y * GetComponent<RectTransform>().rect.height;

            tapToStart.offsetMin = new Vector2(tapToStart.offsetMin.x, bottomPoint + (topPoint - bottomPoint) / 1.7f);
        }


        private void SetTutorialState(Tutorial tutorial)
        {
            currentTutorial = tutorial;

            TutorialManager.Instance.SetFade(true);
            currentTutorial.MaskImage.enabled = true;


            Canvas playerUpgradeCanvas = currentTutorial.Button.gameObject.AddComponent<Canvas>();
            playerUpgradeCanvas.overrideSorting = true;
            playerUpgradeCanvas.sortingLayerName = "UI";
            playerUpgradeCanvas.sortingOrder = 4;
            currentTutorial.Button.gameObject.AddComponent<GraphicRaycaster>();

            currentTutorial.Hand.gameObject.SetActive(true);
            currentTutorial.Button.onClick.AddListener(SetNormalState);
        }


        private void SetNormalState()
        {
            currentTutorial.Hand.gameObject.SetActive(false);
            currentTutorial.MaskImage.enabled = false;

            Destroy(currentTutorial.Button.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(currentTutorial.Button.gameObject.GetComponent<Canvas>());
            Destroy(currentTutorial.Button.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(currentTutorial.Button.gameObject.GetComponent<Canvas>());

            currentTutorial.Panel.SetTutorialState();
            currentTutorial.Button.onClick.RemoveListener(SetNormalState);
        }


        private void SetSubscriptionButtonState()
        {
            bool isSubscriptionActive = (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive);
            buttonSubscription.enabled = !isSubscriptionActive;
            multiImageButtonSubscription.Interactable(!isSubscriptionActive);
            effectButtonSubscription.gameObject.SetActive(!isSubscriptionActive);
        }


        private void Refresh()
        {
            bottomPanel.gameObject.SetActive(TutorialManager.Instance.IsUpgradeAbilityTutorialCanStart || TutorialManager.Instance.IsUpgradeAbilityTutorialPassed);

            if (TutorialManager.Instance.IsUpgradeAbilityTutorialPassed)
            {
                bottomPanel.SetAlert();
            }

            if (TutorialManager.Instance.IsUpgradeWeaponTutorialPassed)
            {
                topPanel.SetAlert();
            }
        }

        #endregion



        #region Events handler

        private void CustomAdvertisingManagerOnBannerVisibilityChanged(bool isVisible)
        {
            topPanelTransform.anchorMax = (isVisible) ? (SaveTopWithBannerOffset) : (SaveTopOffset);
        }
        
        
        private bool StoreManager_PurchaseComplete(IPurchaseItemResult result)
        {
            if (result.IsSucceeded)
            {
                SetSubscriptionButtonState();
            }

            return false;
        }

        #endregion
    }
}
