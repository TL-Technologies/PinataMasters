using Modules.General;
using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Analytics
{
    internal static class CommonGameAnalyticsTracker
    {
        #region Fields

        private const int FirstDayRetentionHoursOffset = 4;
        private const string RetentionEventFormat = "retention_{0}";

        private static readonly List<long> RetentionsDays = new List<long>() { 1, 2 };
        private static float sessionStartTimeInSeconds;

        #endregion



        #region Properties

        private static DateTime FirstLaunchDate
        {
            get => CustomPlayerPrefs.GetDateTime("first_launch", DateTime.MinValue);
            set => CustomPlayerPrefs.SetDateTime("first_launch", value, true);
        }


        private static long CurrentRetentionDay
        {
            get
            {
                DateTime firstLaunchDate = FirstLaunchDate;
                long result = -1;

                if (firstLaunchDate != DateTime.MinValue &&
                    (DateTime.Now - firstLaunchDate).TotalHours >= FirstDayRetentionHoursOffset)
                {
                    DateTime firstLaunchDayDate = new DateTime(firstLaunchDate.Year, firstLaunchDate.Month,
                        firstLaunchDate.Day);
                    long daysDifference = (long)(DateTime.Now - firstLaunchDayDate).TotalDays;

                    if (RetentionsDays.Contains(daysDifference))
                    {
                        result = daysDifference;
                    }
                }

                return result;
            }
        }

        #endregion
        
        
        
        #region Methods

        public static void Initialize()
        {
            TrySendRetentionEvent();
            CommonEvents.SendSessionStart();
            
            LLApplicationStateRegister.OnApplicationEnteredBackground += 
                LLApplicationStateRegister_OnApplicationEnteredBackground;
        }


        private static void TrySendRetentionEvent()
        {
            // long currentRetentionDay = CurrentRetentionDay;
            // string eventName = RetentionEventName(currentRetentionDay);
            //
            // if (currentRetentionDay != -1 && IsRetentionEventAvailable(eventName))
            // {
            //     AnalyticsManager.Instance.SendEvent(eventName);
            //     SaveRetentionEvent(eventName);
            // }
            // else if (FirstLaunchDate == DateTime.MinValue)
            // {
            //     FirstLaunchDate = DateTime.Now;
            // }
        }

        
        private static string RetentionEventName(long day) => String.Format(RetentionEventFormat, day);


        private static bool IsRetentionEventAvailable(string eventName) => !CustomPlayerPrefs.GetBool(eventName);


        private static void SaveRetentionEvent(string eventName) => CustomPlayerPrefs.SetBool(eventName, true);

        #endregion



        #region Events handlers

        private static void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
        {
            if (isEnteredBackground)
            {
                CommonEvents.SendSessionFinish((int)(Time.realtimeSinceStartup - sessionStartTimeInSeconds));
            }
            else
            {
                sessionStartTimeInSeconds = Time.realtimeSinceStartup;
                CommonEvents.SendSessionStart();
            }
        }

        #endregion
    }
}
