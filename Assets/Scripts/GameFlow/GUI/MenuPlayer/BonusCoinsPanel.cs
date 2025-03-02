using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class BonusCoinsPanel : MenuPlayerPanel
    {
        #region Variables

        public static event System.Action<float, string> OnNeedShowMiniBank = delegate { };

        [Header("Content")]
        [SerializeField]
        private TextLocalizator descBonusCoins = null;
        [SerializeField]
        private Button buttonBonusCoins = null;
        [SerializeField]
        private Text priceBonusCoins = null;

        [Header("Vibration")]
        [SerializeField]
        private HapticTypes buttonClickVibrationType = HapticTypes.LightImpact;

        [Header("Tutorial")]
        [SerializeField]
        private RectTransform tutorialHand = null;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            buttonBonusCoins.onClick.AddListener(ButtonBonusCoins);

            Player.OnChangeCoins += Refresh;
        }


        private void Start()
        {
            Refresh();
        }


        private void OnDestroy()
        {
            Player.OnChangeCoins -= Refresh;
        }

        #endregion



        #region Public methods

        public override bool NeedShowAlert()
        {
            return (Player.Coins >= PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel));
        }


        public override void SetTutorialState()
        {
            Canvas ammoUpgradeCanvas = buttonBonusCoins.gameObject.AddComponent<Canvas>();
            ammoUpgradeCanvas.overrideSorting = true;
            ammoUpgradeCanvas.sortingLayerName = "UI";
            ammoUpgradeCanvas.sortingOrder = 4;
            buttonBonusCoins.gameObject.AddComponent<GraphicRaycaster>();

            tutorialHand.gameObject.SetActive(true);

            buttonBonusCoins.onClick.AddListener(SetNormalState);
        }

        #endregion



        #region Private methods

        private void ButtonBonusCoins()
        {
            if (Player.TryRemoveCoins(PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel)))
            {
                VibrationManager.Instance.PlayVibration(buttonClickVibrationType);

                Player.BuyBonusCoinsUpgrade();

                Refresh();
            }
            else
            {
                OnNeedShowMiniBank(PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel) - Player.Coins, MiniBankPlacement.CHARACTER_UPGRADE);
            }
        }


        private void Refresh()
        {
            descBonusCoins.SetParams(PlayerConfig.GetTextBonusCoins());
            priceBonusCoins.text = PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel).ToShortFormat();
            buttonBonusCoins.GetComponent<MultiImageButton>().Interactable(Player.Coins >= PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel));
        }


        private void SetNormalState()
        {
            Destroy(buttonBonusCoins.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(buttonBonusCoins.gameObject.GetComponent<Canvas>());
            tutorialHand.gameObject.SetActive(false);

            TutorialManager.Instance.SetFade(false);
            TutorialManager.Instance.ShowTapToStart();
            TutorialManager.Instance.IsUpgradeAbilityTutorialPassed = true;

            buttonBonusCoins.onClick.RemoveListener(SetNormalState);
        }

        #endregion
    }
}
