using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class SocialController : ScriptableObject
    {
        #region Variables

        private const string PATH_RESOURCES = "Game/SocialController";

        private const string POPUP_INDEX = "social_popup_index";

        private const string LAST_DATE_ACCEPT = "last_accept_social_date"; 
        private const string LAST_DATE_SKIP = "last_skip_social_date";
        private const string PINATAS_KILLED = "pinata_killed_for_social";

        private const int MATCHES_EXCEPT_BOSS_KILLED_FOR_SHOWING = 4;
        private const int MIN_LEVEL_FOR_FIRST_SHOWING = 4;

        [SerializeField]
        private float[] multipliers = null;

        private static SocialController instance;

        #endregion



        #region Properties

        private static SocialController Instance
        {
            get
            {
                instance = instance ?? (SocialController)Resources.Load(PATH_RESOURCES);
                return instance;
            }
        }


        private static int PopUpIndexToPropose
        {
            get
            {
                return CustomPlayerPrefs.GetInt(POPUP_INDEX, 0);
            }

            set
            {
                CustomPlayerPrefs.SetInt(POPUP_INDEX, value);
            }
        }


        public static DateTime LastDateSkip
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_DATE_SKIP, DateTime.Now.AddDays(-1f));
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_DATE_SKIP, value);
            }
        }


        public static DateTime LastDateAccept
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_DATE_ACCEPT, DateTime.Now.AddDays(-1f));
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_DATE_ACCEPT, value);
            }
        }


        private static int MatchesExceptBossKill { get; set; }

        #endregion



        #region Public methods

        public static void Initialize()
        {
            LLPromoFetchManager.Initialize();
            LastDateSkip = DateTime.Now < LastDateSkip ? DateTime.Now : LastDateSkip;
        }


        public static bool CheckForAvailable(out LLPromoFetcherUnit data)
        {
            data = new LLPromoFetcherUnit();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            if (IsAvailableToPropose())
            {
                data = LLPromoFetchManager.PromoFetcherActiveUnit();
#if UNITY_EDITOR
                data.promoURL = "https://www.instagram.com";
#endif
                return !string.IsNullOrEmpty(data.promoURL);
            }

            return false;
        }


        public static void TakeBonus()
        {
            LLPromoFetchManager.VisitActiveUnit(LLPromoVisitType.Opened);
            LastDateAccept = DateTime.Now;
            MatchesExceptBossKill = 0;
            PopUpIndexToPropose++;
        }


        public static void SkipBonus()
        {
            LastDateSkip = DateTime.Now;
            LLPromoFetchManager.VisitActiveUnit(LLPromoVisitType.Skipped);
            MatchesExceptBossKill = 0;
        }


        public static float GetCurrentCoins()
        {
            return Instance.multipliers[PopUpIndexToPropose] * Arsenal.GetWeaponPower(Player.CurrentWeapon);
        }


        public static void TryIncerementMatchesForShowing()
        {
            if (DateTime.Now.Subtract(LastDateSkip).Days > 0)
            {
                MatchesExceptBossKill++;
            }
        }

        #endregion



        #region Private methods

        private static bool IsAvailableToPropose()
        {
            switch (PopUpIndexToPropose)
            {
                case 0:
                    return Player.Level > MIN_LEVEL_FOR_FIRST_SHOWING && (DateTime.Now.Subtract(LastDateSkip).Days > 0)
                           && MatchesExceptBossKill > MATCHES_EXCEPT_BOSS_KILLED_FOR_SHOWING;
                case 1:
                    return (DateTime.Now.Subtract(LastDateAccept).Days > 1) && (DateTime.Now.Subtract(LastDateSkip).Days > 0)
                           && MatchesExceptBossKill > MATCHES_EXCEPT_BOSS_KILLED_FOR_SHOWING;
                case 2:
                    return (DateTime.Now.Subtract(LastDateAccept).Days > 4) && (DateTime.Now.Subtract(LastDateSkip).Days > 0)
                           && MatchesExceptBossKill > MATCHES_EXCEPT_BOSS_KILLED_FOR_SHOWING;
                case 3:
                    return (DateTime.Now.Subtract(LastDateAccept).Days > 9) && (DateTime.Now.Subtract(LastDateSkip).Days > 0)
                           && MatchesExceptBossKill > MATCHES_EXCEPT_BOSS_KILLED_FOR_SHOWING;
                default:
                    return false;
            }
        }

        #endregion
    }
}
