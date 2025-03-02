using Modules.General.Abstraction;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Modules.Notification
{
	public class DemoNotificationController : MonoBehaviour
    {
        #region Fields

        [SerializeField] Button sendMinuteNotificationButton = default;
        [SerializeField] Button sendHourNotificationButton = default;
        [SerializeField] Button sendRepeatableMinuteNotificationButton = default;
        [SerializeField] Button clearNotificationsButton = default;

        #endregion



        #region Unity lifecycle

        private void Start()
        {
            NotificationManager.Instance.Initialize();
        }


        private void OnEnable()
        {
            sendMinuteNotificationButton.onClick.AddListener(SendMinuteNotificationButton_OnClick);
            sendHourNotificationButton.onClick.AddListener(SendHourNotificationButton_OnClick);
            sendRepeatableMinuteNotificationButton.onClick.AddListener(SendRepeatableMinuteNotificationButton_OnClick);
            clearNotificationsButton.onClick.AddListener(ClearNotificationsButton_OnClick);
        }

        
        private void OnDisable()
        {
            sendMinuteNotificationButton.onClick.AddListener(SendMinuteNotificationButton_OnClick);
            sendHourNotificationButton.onClick.AddListener(SendHourNotificationButton_OnClick);
            sendRepeatableMinuteNotificationButton.onClick.AddListener(SendRepeatableMinuteNotificationButton_OnClick);
            clearNotificationsButton.onClick.AddListener(ClearNotificationsButton_OnClick);
        }
        
        #endregion



        #region Events handlers

        private void SendMinuteNotificationButton_OnClick()
        {
            NotificationData notificationData = new NotificationData("testId0", 
                "Test notification text", DateTime.Now.AddMinutes(1), 1);
            NotificationManager.Instance.RegisterLocalNotification(notificationData);
        }
        
        
        private void SendHourNotificationButton_OnClick()
        {
            NotificationData notificationData = new NotificationData("testId1", 
                "Test notification text", DateTime.Now.AddHours(1), 1);
            NotificationManager.Instance.RegisterLocalNotification(notificationData);
        }
        
        
        private void SendRepeatableMinuteNotificationButton_OnClick()
        {
            NotificationData notificationData = new NotificationData("testId2", 
                "Test notification text", DateTime.Now.AddMinutes(1), 7);
            NotificationManager.Instance.RegisterLocalNotification(notificationData);
        }
        
        
        private void ClearNotificationsButton_OnClick()
        {
            NotificationManager.Instance.ClearAllNotifications();
        }

        #endregion
    }
}
