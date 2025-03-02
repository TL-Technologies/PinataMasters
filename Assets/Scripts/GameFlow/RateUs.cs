using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    public static class RateUs
    {
        #region Variables

        private const string WAS_RATED = "was_rated";

#if UNITY_IOS
        private const string RATEUS_URL = "https://itunes.apple.com/app/id1438090112";
#elif UNITY_ANDROID
        private const string RATEUS_URL = "https://play.google.com/store/apps/details?id=com.playgendary.pinatamasters";
#endif
        private const uint levelSpanForShowing = 5;

        private static bool allowShowing;

        private const string LAST_DATE_SHOWED = "last_showed_rate_date";
        private const string WAS_FIRST_POPUP_SHOWED = "was_first_rate_popup_showed";

        #endregion



        #region Properties

        public static string RateUsURL { get { return RATEUS_URL; } }


        public static bool WasRated
        {
            get
            {
                return CustomPlayerPrefs.GetBool(WAS_RATED, false);
            }
            private set
            {
                CustomPlayerPrefs.SetBool(WAS_RATED, value);
            }
        }


        private static DateTime LastDateShow
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_DATE_SHOWED, DateTime.Now.AddDays(-1f));
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_DATE_SHOWED, value);
            }
        }

        private static bool WasFirstPopUpShowed
        {
            get
            {
                return CustomPlayerPrefs.GetBool(WAS_FIRST_POPUP_SHOWED);
            }

            set
            {
                CustomPlayerPrefs.SetBool(WAS_FIRST_POPUP_SHOWED, value);
            }
        }

        #endregion



        #region Public methods

        public static void ShowNativeOrPinataPopUp()
        {
            if (LLRatePopupManager.IsAvalaiblePopUp)
            {
                LLRatePopupManager.ShowPopUp();
                SetAsShowed();
            }
            else
            {
                UIRateUs.Prefab.Instance.Show();
            }
        }


        public static void ShowNativePopUp()
        {
            UIRateUs.Prefab.Instance.Show();
            allowShowing = false;
        }


        public static void AllowToShow()
        {
            allowShowing = true;
        }


        public static bool CanShowFirstPopUp(uint level)
        {
            LastDateShow = DateTime.Now < LastDateShow ? DateTime.Now : LastDateShow;
            return allowShowing && (DateTime.Now.Subtract(LastDateShow).Days > 0) && (level % levelSpanForShowing == 0) &&
                                      Application.internetReachability != NetworkReachability.NotReachable && !WasFirstPopUpShowed;
        }


        public static bool CanShowFollowingPopUp(uint level)
        {
            LastDateShow = DateTime.Now < LastDateShow ? DateTime.Now : LastDateShow;
            return allowShowing && (level % levelSpanForShowing == 0) && (DateTime.Now.Subtract(LastDateShow).Days > 0) &&
                             Application.internetReachability != NetworkReachability.NotReachable && WasFirstPopUpShowed;
        }


        public static void SetAsRated()
        {
            WasRated = true;
        }


        public static void SetAsShowed()
        {
            LastDateShow = DateTime.Now;
            WasFirstPopUpShowed = true;
        }

        #endregion
    }
}
