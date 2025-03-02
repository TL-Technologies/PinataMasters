using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class OfflinerewardPanel : MenuPlayerPanel
    {
        #region Variables

        public static event System.Action<float, string> OnNeedShowMiniBank = delegate { };

        [Header("Content")]
        [SerializeField]
        private TextLocalizator descOfflineReward = null;
        [SerializeField]
        private Button buttonOfflineReward = null;
        [SerializeField]
        private Text priceOfflineReward = null;

        [Header("Vibration")]
        [SerializeField]
        private HapticTypes buttonClickVibrationType = HapticTypes.LightImpact;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            buttonOfflineReward.onClick.AddListener(ButtonOfflineReward);

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
            return (Player.Coins >= PlayerConfig.GetOfflineRewardUpgradePrice(Player.OfflineRewardLevel));
        }

        #endregion



        #region Private methods

        private void ButtonOfflineReward()
        {
            if (Player.TryRemoveCoins(PlayerConfig.GetOfflineRewardUpgradePrice(Player.OfflineRewardLevel)))
            {
                VibrationManager.Instance.PlayVibration(buttonClickVibrationType);

                Player.BuyOfflineRewardUpgrade();

                Refresh();
            }
            else
            {
                OnNeedShowMiniBank(PlayerConfig.GetOfflineRewardUpgradePrice(Player.OfflineRewardLevel) - Player.Coins, MiniBankPlacement.CHARACTER_UPGRADE);
            }
        }


        private void Refresh()
        {
            descOfflineReward.SetParams(PlayerConfig.GetTextBonusOfflineReward(Player.OfflineRewardLevel));
            priceOfflineReward.text = PlayerConfig.GetOfflineRewardUpgradePrice(Player.OfflineRewardLevel).ToShortFormat();
            buttonOfflineReward.GetComponent<MultiImageButton>().Interactable(Player.Coins >= PlayerConfig.GetOfflineRewardUpgradePrice(Player.OfflineRewardLevel));
        }

        #endregion
    }
}
