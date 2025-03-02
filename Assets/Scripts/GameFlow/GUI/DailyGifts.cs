using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class DailyGifts : ScriptableObject
    {
        #region Fields

        private static readonly ResourceAsset<DailyGifts> asset = new ResourceAsset<DailyGifts>("Game/DailyGifts");

        private const string LAST_DATE_GET = "last_get_gift_date";
        private const string DAILY_GIFT_DAY = "daily_gift_day";

        [SerializeField]
        private DailyGiftConfig[] rewardConfig = null;
        [SerializeField]
        private DailyGiftData[] rewards = null;

        #endregion



        #region Properties

        private static DateTime LastDateGet
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_DATE_GET, DateTime.Now.AddDays(-1f));
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_DATE_GET, value);
            }
        }


        public static int DailyGiftDay
        {
            get
            {
                return CustomPlayerPrefs.GetInt(DAILY_GIFT_DAY, 0) % asset.Value.rewards.Length;
            }
            private set
            {
                CustomPlayerPrefs.SetInt(DAILY_GIFT_DAY, value % asset.Value.rewards.Length);
            }
        }


        public static bool IsGiftAvailable
        {
            get
            {
                 LastDateGet = DateTime.Now < LastDateGet ? DateTime.Now : LastDateGet;

                return DateTime.Now.Subtract(LastDateGet).Days > 0;
            }
        }

        #endregion



        #region Public methods

        public static void TakeGift()
        {
            LastDateGet = DateTime.Now;
            GameAnalytics.SendGiftClaimEvent(DailyGiftDay + 1);
            DailyGiftDay++;
        }


        public static float GetCoins(int day)
        {
            return Arsenal.GetWeaponPower(Player.CurrentWeapon) * asset.Value.rewards[day].Multiplier;
        }


        public static DailyGiftConfig GetConfigsByDay(int day)
        {
            TypeReward type = asset.Value.rewards[day].Type;
            DailyGiftConfig config = Array.Find(asset.Value.rewardConfig, c => c.Type == type);
            return config;
        }


        public static DateTime GetTime()
        {
            return DateTime.Now + new TimeSpan(24, 0, 0);
        }

        #endregion
    }
}
