using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class PlayerConfig : ScriptableObject
    {

        #region Variables

        private const float HalfPI = Mathf.PI * 0.5f;
        private const string PATH_RESOURCES = "Game/PlayerConfig";

        [Header("Prestige")]
        [SerializeField]
        private uint minLevelForReset = 25;

        [Header("Speed")]
        [SerializeField]
        private float speed                     = 4f;
        [SerializeField]
        private float speedMax                  = 16f;
        [SerializeField]
        private float speedUpgradePrice          = 55f;

        [Header("OfflineReward")]
        [SerializeField]
        private float offlineRewardUpgrade      = 0.02f;
        [SerializeField]
        private float offlineRewardUpgradePrice  = 55f;

        [Header("BonusCoins")]
        [SerializeField]
        private float bonusCoinsUpgrade         = 0.04f;
        [SerializeField]
        private float bonusCoinsUpgradePrice     = 55f;


        private static PlayerConfig instance;

        #endregion



        #region Properties

        private static PlayerConfig i
        {
            get
            {
                instance = instance ?? (PlayerConfig)Resources.Load(PATH_RESOURCES);
                return instance;
            }
        }

        #endregion



        #region Public methods

        public static float GetResultCoins(float resultCoins)
        {
            return resultCoins + resultCoins * GetBonusCoins(Player.BonusCoinsLevel);
        }


        public static uint GetTextBonusCoins()
        {
            return (uint)Mathf.RoundToInt(GetBonusCoins(Player.BonusCoinsLevel) * 100f);
        }


        public static float GetBonusCoinsUpgradePrice(uint bonusCoinsLevel)
        {
            return i.bonusCoinsUpgradePrice * Mathf.Pow(1.7f, bonusCoinsLevel);
        }


        public static float GetOfflineReward(float deltaTimerMinutes)
        {
            return deltaTimerMinutes * Arsenal.GetWeaponPower(Player.CurrentWeapon) / 60f * (1f + GetOfflineRewardMultiplier(Player.OfflineRewardLevel));
        }


        public static float GetTextBonusOfflineReward(uint offlineRewardLevel)
        {
            return Mathf.RoundToInt(offlineRewardLevel * i.offlineRewardUpgrade * 100f);
        }


        public static float GetOfflineRewardUpgradePrice(uint offlineRewardLevel)
        {
            return i.offlineRewardUpgradePrice * Mathf.Pow(1.7f, offlineRewardLevel);
        }


        public static float GetOfflineRewardMultiplier(uint offlineRewardLevel)
        {
            return offlineRewardLevel * i.offlineRewardUpgrade;
        }


        public static float GetSpeed(uint speedLevel)
        {
            return i.speed + GetBonusSpeed(speedLevel);
        }


        public static uint GetTextSpeed(uint speedLevel)
        {
            return (uint)Mathf.RoundToInt(GetBonusSpeed(speedLevel) / i.speed * 100f);
        }


        public static float GetSpeedUpgradePrice(uint speedLevel)
        {
            return i.speedUpgradePrice * Mathf.Pow(1.7f, speedLevel);
        }


        public static float GetPrestigeReward()
        {
            return SelectorLevels.GetLevels.GetGemsForReset(Player.Level);
        }


        public static float GetMinPrestigeReward()
        {
            return SelectorLevels.GetLevels.GetGemsForReset(GetMinLevelForReset());
        }


        public static bool IsResetAllow()
        {
            return Player.Level >= i.minLevelForReset;
        }


        public static uint GetMinLevelForReset()
        {
            return i.minLevelForReset;
        }

        #endregion



        #region Private methods

        public static float GetBonusCoins(uint bonusCoinsLevel) 
        {
            return bonusCoinsLevel * i.bonusCoinsUpgrade;
        }


        private static float GetBonusSpeed(uint speedLevel)
        {
            float bonus = i.speedMax - i.speed;
            float prs = Mathf.Atan(speedLevel / 25f) / HalfPI;
            return bonus * prs;
        }

        #endregion

    }
}
