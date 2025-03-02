using Modules.General;
using System;
#if UNITY_IOS
    using Unity.Notifications.iOS;
#elif UNITY_ANDROID
    using Unity.Notifications.Android;
#endif


namespace Modules.Notification
{
    public static class UnityNotificationManager
    {
        #region Public methods

        public static void Initialize(Action onCompleteCallback = null)
        {
            #if UNITY_EDITOR
                onCompleteCallback?.Invoke();
            #elif UNITY_IOS
                SendAuthorizationRequest().ContinueWith(task =>
                {
                    // We should return callback invocation to main thread
                    Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance, () =>
                    {
                        onCompleteCallback?.Invoke();
                    }, 0f);
                }); 
            #elif UNITY_ANDROID
                RegisterNotificationChannel();
                onCompleteCallback?.Invoke();
            #endif
        }
             
             
        public static void ClearAllNotifications()
        {
            #if UNITY_IOS
                iOSNotificationCenter.RemoveAllScheduledNotifications();
            #elif UNITY_ANDROID
                AndroidNotificationCenter.CancelAllNotifications();
            #endif
        }
             
             
        public static void ClearNotification(string identifier)
        {   
            #if UNITY_IOS
                iOSNotificationCenter.RemoveScheduledNotification(identifier);
            #elif UNITY_ANDROID
                AndroidNotificationCenter.CancelNotification(identifier.ParseInt());
            #endif
        }
             
             
        public static string ScheduleLocalNotification(string notificationId, string title,
            string text, DateTime fireDate, string intentData)
        {
            UnityNotificationSettings settings = UnityNotificationSettings.Instance;
            string identifier = string.Empty;

            #if UNITY_IOS
                DateTime currentDate = DateTime.Now;
                TimeSpan targetTimeSpan = fireDate.Subtract(currentDate);
                        
                iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger(); 
                timeTrigger.TimeInterval = targetTimeSpan;
                
                iOSNotification notification = new iOSNotification() 
                { 
                    Identifier = notificationId,
                    Title = title, 
                    Body = text, 
                    ShowInForeground = settings.ShowInForeground, 
                    ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge), 
                    Trigger = timeTrigger,
                };
                
                identifier = notificationId; 
                iOSNotificationCenter.ScheduleNotification(notification);
            #elif UNITY_ANDROID
                AndroidNotification notification = new AndroidNotification()
                {
                    Title     = title,
                    Text      = text,
                    SmallIcon = settings.SmallIconName,
                    LargeIcon = settings.LargeIconName,
                    FireTime  = fireDate,
                    IntentData = intentData
                };

                identifier = AndroidNotificationCenter.SendNotification(notification, settings.ChannelId).ToString();
            #endif
                 
            return identifier;
        }


        #if UNITY_ANDROID
        
        public static AndroidNotificationIntentData GetLastNotificationIntent() => AndroidNotificationCenter.GetLastNotificationIntent();
        
        
        public static void RegisterNotificationChannel()
        {
            UnityNotificationSettings settings = UnityNotificationSettings.Instance;
                
            AndroidNotificationChannel defaultNotificationChannel = new AndroidNotificationChannel() 
            { 
                Id = settings.ChannelId, 
                Name = settings.ChannelName, 
                Importance = Importance.Default, 
                Description = settings.ChannelDescription
            };
            
            AndroidNotificationCenter.RegisterNotificationChannel(defaultNotificationChannel);
        }
        
        #elif UNITY_IOS && !UNITY_EDITOR
        
        public static async System.Threading.Tasks.Task SendAuthorizationRequest(Action<bool> authorizationCallback = null)
        {
                using (var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
                {
                    while (!request.IsFinished)
                    {
                        await System.Threading.Tasks.Task.Yield();
                    };
                    
                    authorizationCallback?.Invoke(request.Granted);
                }
        }

        #endif
        #endregion
    }    
}
