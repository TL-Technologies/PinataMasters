using Modules.General;
using Modules.General.HelperClasses;
using Modules.Notification.Obsolete;
using System;
using System.Collections.Generic;


#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace PinataMasters
{
    public static class Notification
    {
        #region Variables

        const string keyOfflineReward = "NOTIFICATION_OFFLINE_REWARD";
        const string keyDailyGift = "NOTIFICATION_DAILY_GIFT";

        const string OFFLINE_REWARD_TEXT_KEY = "localnotification.offlinereward";
        const string DAILY_GIFT_TEXT_KEY = "localnotification.dailygift";

        const int MAX_NOTIFICATION_HOUR = 23;
        const int MIN_NOTIFICATION_HOUR = 10;
        const int HOURS_IN_DAY = 24;

        #endregion

        public static void QueryNotification()
        {
            #if UNITY_IOS
            if (!CustomPlayerPrefs.GetBool("NOTIFICATION", false))
            {
                NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound, false);
                CustomPlayerPrefs.SetBool("NOTIFICATION", true);
            }
            #endif
        }

        public static void Init()
        {
            Unregister();
            LLNotificationManager.Init();
            LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;
        }

        private static void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
        {
            if (isEnteredBackground)
            {
                Register();
            }
            else
            {
                Unregister();
            }
        }

        private static void Unregister()
        {
            UnregisteLocalNotifications(keyOfflineReward);
            UnregisteLocalNotifications(keyDailyGift);
        }

        private static void Register()
        {
            RegisteLocalNotification(keyOfflineReward, Localisation.LocalizedStringOrSource(OFFLINE_REWARD_TEXT_KEY),
                OfflineReward.GetFullReward(), false);
            RegisteLocalNotification(keyDailyGift, Localisation.LocalizedStringOrSource(DAILY_GIFT_TEXT_KEY), DailyGifts.GetTime(), true);
        }

        private static void RegisteLocalNotification(string notificationKey, string text, DateTime date, bool isRepeatEveryDay = false)
        {
            date = VerifyDate(date);
            #if UNITY_IOS
                LocalNotification localNotification = new LocalNotification
                {
                    alertBody = text,
                    fireDate = date,
                    soundName = LocalNotification.defaultSoundName,
                    userInfo = new Dictionary<string, string> { { notificationKey, text } }
                };

                if (isRepeatEveryDay)
                {
                    localNotification.repeatInterval = CalendarUnit.Day;
                }
                
                NotificationServices.ScheduleLocalNotification(localNotification);
            #elif UNITY_ANDROID
                int iterations = (isRepeatEveryDay) ? 14 : 1;
                LLNotificationManager.ScheduleLocalNotification(notificationKey, text,
                    UnityEngine.Application.productName,  text, date, iterations);
            #endif
        }


        private static void UnregisteLocalNotifications(string notificationKey)
        {
            #if UNITY_IOS
                if (NotificationServices.scheduledLocalNotifications != null)
                {
                    foreach (LocalNotification notification in NotificationServices.scheduledLocalNotifications)
                    {
                        if (notification != null && notification.userInfo != null && notification.userInfo.Contains(notificationKey))
                        {
                            NotificationServices.CancelLocalNotification(notification);
                        }
                    }
                }
            #elif UNITY_ANDROID
                LLNotificationManager.CancelAllLocalNotification(notificationKey);
            #endif
        }

        private static DateTime VerifyDate(DateTime nextNotificationTime)
        {
            DateTime result = nextNotificationTime;

            if (nextNotificationTime.Hour < MIN_NOTIFICATION_HOUR)
            {
                result = result.AddHours(MIN_NOTIFICATION_HOUR - result.Hour);
            }
            else if (nextNotificationTime.Hour > MAX_NOTIFICATION_HOUR)
            {
                result = result.AddHours(MIN_NOTIFICATION_HOUR - result.Hour + HOURS_IN_DAY);
            }

            return result;
        }
    }
}
