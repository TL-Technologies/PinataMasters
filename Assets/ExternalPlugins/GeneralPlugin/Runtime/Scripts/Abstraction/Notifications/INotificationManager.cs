using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface INotificationManager
    {
        bool AreNotificationsEnabled { get; set; }
        Dictionary<string, List<string>> RegisteredNotificationsIdentifiers { get; }

        void Initialize();
        bool RegisterLocalNotification(NotificationData notificationData);
        void UnregisterLocalNotification(string notificationId);
        void ClearAllNotifications();
    }
}

