using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class SpeedPanel : MenuPlayerPanel
    {
        #region Variables

        public static event System.Action<float, string> OnNeedShowMiniBank = delegate { };

        [Header("Content")]
        [SerializeField]
        private TextLocalizator descSpeed = null;
        [SerializeField]
        private Button buttonSpeed = null;
        [SerializeField]
        private Text priceSpeed = null;

        [Header("Vibration")]
        [SerializeField]
        private HapticTypes buttonClickVibrationType = HapticTypes.LightImpact;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            buttonSpeed.onClick.AddListener(ButtonSpeed);

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
            return (Player.Coins >= PlayerConfig.GetSpeedUpgradePrice(Player.SpeedLevel));
        }

        #endregion



        #region Private methods

        private void ButtonSpeed()
        {
            if (Player.TryRemoveCoins(PlayerConfig.GetSpeedUpgradePrice(Player.SpeedLevel)))
            {
                VibrationManager.Instance.PlayVibration(buttonClickVibrationType);

                Player.BuySpeedUpgrade();

                Refresh();
            }
            else
            {
                OnNeedShowMiniBank(PlayerConfig.GetSpeedUpgradePrice(Player.SpeedLevel) - Player.Coins, MiniBankPlacement.CHARACTER_UPGRADE);
            }
        }


        private void Refresh()
        {
            descSpeed.SetParams(Mathf.RoundToInt(PlayerConfig.GetTextSpeed(Player.SpeedLevel)));
            priceSpeed.text = PlayerConfig.GetSpeedUpgradePrice(Player.SpeedLevel).ToShortFormat();
            buttonSpeed.GetComponent<MultiImageButton>().Interactable(Player.Coins >= PlayerConfig.GetSpeedUpgradePrice(Player.SpeedLevel));
        }

        #endregion
    }
}
