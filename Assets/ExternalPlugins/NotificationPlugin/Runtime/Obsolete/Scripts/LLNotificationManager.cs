using System;


namespace Modules.Notification.Obsolete
{
    public sealed class LLNotificationManager
    {
        #region Fields

        #if UNITY_ANDROID
            private const string MethodInit = "LLNotificationInit";
            private const string MethodScheduleLocalNotification = "LLNotificationScheduleLocalNotification";
            private const string MethodCancelAllLocalNotificationWithKey = "LLNotificationCancelAllLocalNotificationWithKey";
        #endif

        private static string notificationIconTextureName;
        private static string viewIconTextureName;
        private static string viewBackgroundTextureName;
        private static int titleColor;
        private static int descriptionColor;

        #endregion



        #region Methods

        public static void Init()
        {
            notificationIconTextureName = LLNotificationSettings.Instance.NotificationIconTexture != null ?
                LLNotificationSettings.Instance.NotificationIconTexture.name : null;
            viewIconTextureName = LLNotificationSettings.Instance.ViewIconTexture != null ? 
                LLNotificationSettings.Instance.ViewIconTexture.name : null;
            viewBackgroundTextureName = LLNotificationSettings.Instance.ViewBackgroundTexture != null ? 
                LLNotificationSettings.Instance.ViewBackgroundTexture.name : null;
            titleColor = LLNotificationSettings.Instance.TitleColor.ToInt();
            descriptionColor = LLNotificationSettings.Instance.DescriptionColor.ToInt();

            #if UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLNotificationManager>.CallStatic(MethodInit);
            #endif
        }


        [Obsolete("Use Modules.Notification.Obsolete.NotificationDataBuilder")]
        public static void ScheduleLocalNotification(string notificationKey, string notificationId, string title,
            string description, DateTime fireDate, int daysRepeat = 1)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLNotificationManager>.CallStatic(MethodScheduleLocalNotification,
                    notificationKey, notificationId, notificationIconTextureName, viewIconTextureName, viewBackgroundTextureName, title, titleColor,
                    description, descriptionColor, fireDate.ToString("G"), daysRepeat, null);
            #endif
        }


        public static void ScheduleLocalNotification(NotificationData data)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLNotificationManager>.CallStatic(MethodScheduleLocalNotification,
                    data.Key, data.Id, notificationIconTextureName, viewIconTextureName, viewBackgroundTextureName, data.Title, titleColor,
                    data.Description, descriptionColor, data.FireDate.ToString("G"), data.DaysRepeat,
                    data.CustomViewFactoryClassName);
            #endif
        }


        public static void CancelAllLocalNotification(string notificationKey)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                LLAndroidJavaSingletone<LLNotificationManager>.CallStatic(MethodCancelAllLocalNotificationWithKey, notificationKey);
            #endif
        }

        #endregion
    }
}
