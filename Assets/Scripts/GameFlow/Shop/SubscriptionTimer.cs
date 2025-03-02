using MiniJSON;
using Modules.General;
using Modules.General.HelperClasses;
using System;
using System.Collections;
using UnityEngine;


namespace PinataMasters
{
    public static class SubscriptionTimer
    {
        #region Variables

        private const string LAST_REAL_UTC_DATE = "last_real_utc_date";
        private const string IS_LAST_SUBSCRIPTION_ACTIVE = "is_last_subscription_active";

        private const float SUBSCRIPTION_TIMEOUT = 0.5f;

        private static string SERVER_TIME_URL = "https://api.playgendary.com/v1/info/time?build=";
        private const string TIME_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";

        private const float bottomSecondsDifferenceForTimeWarning = -120f;
        private const float topSecondsDifferenceForTimeWarning = 120f;

        private static TimeSpan timeOffset;
        private static bool isCheckTimeCoroutineStarted;
        private static bool isServerTimeReceived;

        private static bool IsLastSubscritionChangingAvailable = true;

        #endregion



        #region Properties

        public static DateTime DateToCheck
        {
            get
            {
                DateTime lastRealDate = LastRealUtcDate;
                DateTime currentDate = DateTime.UtcNow.Subtract(CheatedTime);

                if (lastRealDate == DateTime.MinValue)
                {
                    return currentDate;
                }
                else
                {
                    int dateCompare = DateTime.Compare(currentDate, lastRealDate);
                    return (dateCompare < 0) ? lastRealDate : currentDate;
                }
            }
        }


        public static bool IsLastSubscriptionActive
        {
            get
            {
                return CustomPlayerPrefs.GetBool(IS_LAST_SUBSCRIPTION_ACTIVE, false);
            }
            set
            {
                if (!IsLastSubscritionChangingAvailable)
                {
                    CustomPlayerPrefs.SetBool(IS_LAST_SUBSCRIPTION_ACTIVE, value);
                }
            }
        }


        private static DateTime CurrentTime
        {
            get
            {
                if (isServerTimeReceived)
                {
                    return DateTime.Now + timeOffset;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }


        private static DateTime LastRealUtcDate
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_REAL_UTC_DATE, DateTime.MinValue);
            }

            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_REAL_UTC_DATE, value);
            }
        }


        private static TimeSpan CheatedTime { get; set; }

        #endregion



        #region Unity lifecycle

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            Sheduler.PlayCoroutine(CheckServerTime());
            Sheduler.PlayCoroutine(SetLastSubscriptionChangeAvailable());

            LLApplicationStateRegister.OnApplicationEnteredBackground += TryCheckServerTime;
        }

        #endregion



        #region Private methods

        private static IEnumerator SetLastSubscriptionChangeAvailable()
        {
            yield return new WaitForSeconds(SUBSCRIPTION_TIMEOUT);
            IsLastSubscritionChangingAvailable = false;
        }


        private static IEnumerator CheckServerTime()
        {
            isCheckTimeCoroutineStarted = true;

            WWW info = new WWW(SERVER_TIME_URL + Application.version);
            yield return info;

            if (info.error != null)
            {
                isServerTimeReceived = false;
            }
            else
            {
                string serverInfo = JsonConvert.DeserializeObject<string>(info.text);
                if (serverInfo != null)
                {
                    CheatedTime = DateTime.Now - DateTime.ParseExact(serverInfo, TIME_FORMAT, null);
                    LastRealUtcDate = DateTime.UtcNow.Subtract(CheatedTime);

                    timeOffset = DateTime.ParseExact(serverInfo, TIME_FORMAT, null) - DateTime.Now;
                    isServerTimeReceived = true;

                    float offsetSeconds = (float)timeOffset.TotalSeconds;
                    if (offsetSeconds <= bottomSecondsDifferenceForTimeWarning ||
                        offsetSeconds >= topSecondsDifferenceForTimeWarning)
                    {
                        GameAnalytics.SetCheaterUserProperty();
                    }

                    if (offsetSeconds < 0f)
                    {
                        timeOffset = new TimeSpan(0, 0, 0);
                    }
                }
                else
                {
                    isServerTimeReceived = false;
                }
            }

            info.Dispose();
            isCheckTimeCoroutineStarted = false;
        }


        private static void TryCheckServerTime(bool isEnterBackground)
        {
            if (isEnterBackground)
            {
                isServerTimeReceived = false;
            }
            else
            {
                if (!isCheckTimeCoroutineStarted)
                {
                    Sheduler.PlayCoroutine(CheckServerTime());
                }
            }
        }

        #endregion
    }
}
