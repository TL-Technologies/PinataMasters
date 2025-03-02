using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Notification
{
    [InitQueueService(6, bindTo: typeof(INotificationManager))]
    public class NotificationManager : INotificationManager, IInitializableService
    {
        #region Fields

        private const string AreNotificationsEnabledKey = "are_notifications_enabled";
        private const int MaxNotificationHour = 22;
        private const int MinNotificationHour = 10;
        private const int HoursInDay = 24;

        public float minHoursBetweenNotifications = 1;

        private bool areNotificationsEnabled = true;
        private static INotificationManager instance;
        private List<NotificationData> registeredNotifications = new List<NotificationData>();
        private Dictionary<string, List<string>> registeredNotificationsIdentifiers = new Dictionary<string, List<string>>();
        private List<DateTime> notificationDates = new List<DateTime>();

        private Action onCompleteCallback;

        #endregion



        #region Properties

        public bool AreNotificationsEnabled
        {
            get => areNotificationsEnabled;
            set
            {
                if (areNotificationsEnabled != value)
                {
                    areNotificationsEnabled = value;
                    CustomPlayerPrefs.SetBool(AreNotificationsEnabledKey, areNotificationsEnabled);

                    #if UNITY_IOS && !UNITY_EDITOR

                        if (value && UnityNotificationSettings.Instance.AutoSendRequest)
                        {
                            UnityNotificationManager.SendAuthorizationRequest(); 
                        }

                    #endif
                }
            }
        }


        public Dictionary<string, List<string>> RegisteredNotificationsIdentifiers => registeredNotificationsIdentifiers;


        public static INotificationManager Instance => instance ?? (instance = new NotificationManager());

        #endregion



        #region Methods

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            this.onCompleteCallback = onCompleteCallback;

            Initialize();
        }


        public void Initialize()
        {
            LLApplicationStateRegister.OnApplicationEnteredBackground +=
                LLApplicationStateRegister_OnApplicationEnteredBackground;

            areNotificationsEnabled = CustomPlayerPrefs.GetBool(AreNotificationsEnabledKey, UnityNotificationSettings.Instance.DefaultEnabledNotifications);

            if (AreNotificationsEnabled)
            {
                UnityNotificationManager.Initialize(onCompleteCallback);
                ClearAllNotifications();
            }
            else
            {
                onCompleteCallback?.Invoke();
            }
            onCompleteCallback = null;
        }


        public bool RegisterLocalNotification(NotificationData notificationData)
        {
            bool notificationWasRegistered = false;
            DateTime date = VerifyDate(notificationData.fireDate);
            notificationData.fireDate = date;

            if (CanRegisterNotificationIdentifier(date, notificationData.notificationId))
            {
                for (int i = 0; i < notificationData.notificationShowCount; i++)
                {
                    DateTime notificationDate = date.AddDays(i);

                    if (CanRegisterNotificationForDate(notificationDate, notificationData.notificationId))
                    {
                        notificationWasRegistered = true;
                        notificationDates.Add(notificationDate);
                    }
                }

                if (notificationWasRegistered)
                {
                    RegisteredNotificationsIdentifiers.Add(notificationData.notificationId, new List<string>());
                    registeredNotifications.Add(notificationData);
                }
            }

            return notificationWasRegistered;
        }


        public void UnregisterLocalNotification(string notificationId)
        {
            if (RegisteredNotificationsIdentifiers.ContainsKey(notificationId))
            {
                foreach (string identifier in RegisteredNotificationsIdentifiers[notificationId])
                {
                    UnityNotificationManager.ClearNotification(identifier);
                }

                RegisteredNotificationsIdentifiers.Remove(notificationId);
            }

            for (int i = 0; i < registeredNotifications.Count; i++)
            {
                if (registeredNotifications[i].notificationId == notificationId)
                {
                    notificationDates.Remove(registeredNotifications[i].fireDate);
                    registeredNotifications.RemoveAt(i);
                    i--;
                }
            }
        }


        public void ClearAllNotifications()
        {
            notificationDates.Clear();
            registeredNotifications.Clear();
            RegisteredNotificationsIdentifiers.Clear();
            UnityNotificationManager.ClearAllNotifications();
        }


        private void ScheduleLocalNotification(NotificationData notificationData)
        {
            List<string> notificationIds = RegisteredNotificationsIdentifiers[notificationData.notificationId];

            string notificationTitle = !string.IsNullOrEmpty(notificationData.notificationTitle) ?
                notificationData.notificationTitle :
                Application.productName;

            for (int i = 0; i < notificationData.notificationShowCount; i++)
            {
                DateTime notificationDate = notificationData.fireDate.AddDays(i);

                if (CanRegisterNotificationForDate(notificationDate, notificationData.notificationId))
                {
                    string identifier = UnityNotificationManager.ScheduleLocalNotification(
                        $"{notificationData.notificationId}_{i}",
                        notificationTitle,
                        notificationData.notificationText,
                        notificationDate,
                        notificationData.intentData);

                    notificationIds.Add(identifier);
                    notificationDates.Add(notificationDate);
                }
            }
        }


        private void ScheduleAllLocalNotifications()
        {
            foreach (NotificationData registeredNotification in registeredNotifications)
            {
                ScheduleLocalNotification(registeredNotification);
            }
        }


        private DateTime VerifyDate(DateTime nextNotificationTime)
        {
            DateTime result = nextNotificationTime;

            if (nextNotificationTime.Hour < MinNotificationHour)
            {
                result = result.AddHours(MinNotificationHour - result.Hour);
                FindEmptyDateTime(ref result);
            }
            else if (nextNotificationTime.Hour >= MaxNotificationHour)
            {
                result = result.AddHours(MinNotificationHour - result.Hour + HoursInDay);
                FindEmptyDateTime(ref result);
            }

            return result;
        }


        private bool CanRegisterNotificationIdentifier(DateTime dateTime, string notificationId)
        {
            bool result = true;

            if (RegisteredNotificationsIdentifiers.ContainsKey(notificationId))
            {
                result = false;
                CustomDebug.LogWarning($"Notification id already exists, notification {notificationId} won't be set!");
            }

            return result;
        }


        private bool CanRegisterNotificationForDate(DateTime dateTime, string notificationId)
        {
            bool result = true;

            if (dateTime <= DateTime.Now)
            {
                result = false;
                CustomDebug.LogWarning($"Notification date {dateTime} is less than {DateTime.Now}, notification {notificationId} won't be set!");
            }
            else if (dateTime >= DateTime.Now.AddYears(1))
            {
                result = false;
                CustomDebug.LogWarning($"Notification date is to big, notification {notificationId} won't be set!");
            }
            else
            {
                foreach (DateTime notificationDate in notificationDates)
                {
                    double hoursDelta = dateTime.Subtract(notificationDate).TotalHours;

                    if (Math.Abs(hoursDelta) < minHoursBetweenNotifications)
                    {
                        result = false;
                        CustomDebug.LogWarning($"Time between notifications less, then minimal, notification {notificationId} won't be set!");

                        break;
                    }
                }
            }

            return result;
        }


        private bool FindEmptyDateTime(ref DateTime dateTime)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            DateTime tempDateTime = dateTime;

            foreach (DateTime notificationDate in notificationDates)
            {
                if (notificationDate.Day == dateTime.Day && notificationDate.Month == dateTime.Month)
                {
                    dateTimes.Add(notificationDate);
                }
            }

            bool result = FindEmptyHour(dateTimes, ref tempDateTime) && (tempDateTime.Hour < MaxNotificationHour);

            if (result)
            {
                dateTime = tempDateTime;
            }

            return result;
        }


        private bool FindEmptyHour(List<DateTime> dateTimes, ref DateTime dateTime)
        {
            bool result = true;

            foreach (DateTime date in dateTimes)
            {
                double hoursDelta = dateTime.Subtract(date).TotalHours;

                if (Math.Abs(hoursDelta) < minHoursBetweenNotifications)
                {
                    dateTime = dateTime.AddHours(minHoursBetweenNotifications);

                    if (date.Day == dateTime.Day)
                    {
                        result = FindEmptyHour(dateTimes, ref dateTime);
                    }
                    else
                    {
                        result = false;

                        break;
                    }
                }
            }

            return result;
        }

        #endregion



        #region Events handler

        private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
        {
            UnityNotificationManager.ClearAllNotifications();
            notificationDates.Clear();

            if (isEnteredBackground && AreNotificationsEnabled)
            {
                ScheduleAllLocalNotifications();
            }
        }

        #endregion
    }
}
