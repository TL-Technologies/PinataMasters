using Modules.General;
using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    public class OfflineReward
    {
        #region Variables

        private const int OFFLINE_MINUTES_LIMIT = 180;

        private const uint SIMPLE_MULTIPLIER = 1u;
        private const uint SIMPLE_SUBSCRIPTION_MULTIPLIER = 2u;

        private const uint BONUS_MULTIPLIER = 1u;
        private const uint BONUS_SUBSCRIPTION_MULTIPLIER = 2u;

        private const string LAST_UTC_DATE = "LAST_UTC_DATE";
        private const string OFFLINE_COINS_BANK = "OFFLINE_COINS_BANK";

        #endregion



        #region Properties

        private static DateTime LastUtcDate
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_UTC_DATE, DateTime.UtcNow);
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_UTC_DATE, value);
            }
        }

        public static float OfflineCoinsBank
        {
            get
            {
                return CustomPlayerPrefs.GetFloat(OFFLINE_COINS_BANK, 0f);
            }

            private set
            {
                CustomPlayerPrefs.SetFloat(OFFLINE_COINS_BANK, value);
            }
        }

        #endregion



        static OfflineReward()
        {
            LLApplicationStateRegister.OnApplicationEnteredBackground += OnGameFocus;

            AddCoinsToBank();
        }


        #region Public methods

        public static uint GetSimpleMultiplier(bool isSubscriptionActive)
        {
            return (isSubscriptionActive) ? (SIMPLE_SUBSCRIPTION_MULTIPLIER) : (SIMPLE_MULTIPLIER);
        }

       
        public static uint GetBonusMultiplier(bool isSubscriptionActive)
        {
            return (uint)ABTest.InGameAbTestData.offlineRewardMultiplier * ((isSubscriptionActive) ? (BONUS_SUBSCRIPTION_MULTIPLIER) : (BONUS_MULTIPLIER));
        }


        public static DateTime GetFullReward()
        {
            return DateTime.Now + new TimeSpan(0, OFFLINE_MINUTES_LIMIT, 0);
        }


        public static float TakeOfflineReward()
        {
            float coins = OfflineCoinsBank;
            GameAnalytics.SendClaimRewardEvent();
            return coins;
        }


        public static void ResetOfflineCoinsBank()
        {
            OfflineCoinsBank = 0f;
        }

        #endregion



        #region Private methods

        private static void AddCoinsToBank()
        {
            if (DateTime.UtcNow < LastUtcDate)
            {
                SetLastGameDate();
            }

            float deltaTimerMinutes = Mathf.Clamp((float)(DateTime.UtcNow - LastUtcDate).TotalMinutes, 0f, (float)OFFLINE_MINUTES_LIMIT);
            OfflineCoinsBank += PlayerConfig.GetOfflineReward(deltaTimerMinutes);
        }


        private static void OnGameFocus(bool isGameUnFocus)
        {
            if (isGameUnFocus)
            {
                SetLastGameDate();
            }
            else
            {
                AddCoinsToBank();
            }
        }


        private static void SetLastGameDate()
        {
            LastUtcDate = DateTime.UtcNow;
        }

        #endregion
    }
}