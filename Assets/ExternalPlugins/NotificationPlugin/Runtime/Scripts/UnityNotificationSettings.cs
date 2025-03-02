using Modules.General.HelperClasses;
using UnityEngine;


namespace Modules.Notification
{
	[CreateAssetMenu(fileName = "UnityNotificationSettings")]
	public class UnityNotificationSettings : ScriptableSingleton<UnityNotificationSettings>
	{
		#region Fields

		[Header("Base", order = -1)] 
		[SerializeField] private bool defaultEnabledNotifications = true;

		[Header("Android", order = 0)]
		[Header("Icon settings", order = 1)]
		[SerializeField] private string smallIconName = "app_icon_small";
		[SerializeField] private string largeIconName = "app_icon_large";
    
		[Space][Space]
		[Header("Ios", order = 1)]
		[Header("Notification settings", order = 2)]
		[SerializeField] private bool showInForeground = true;
		[SerializeField] private bool autoSendRequest = true;
		
		private const string NotificationChannelId = "default_channel";
		private const string NotificationChannelDescription = "All application android notifications";
		private const string NotificationChannelName = "Application Notifications";
    
		#endregion



		#region Properties

		public string ChannelId => NotificationChannelId;
    
    
		public string ChannelDescription => NotificationChannelDescription;


		public string ChannelName => NotificationChannelName;


		public string SmallIconName => smallIconName;

    
		public string LargeIconName => largeIconName;


		public bool ShowInForeground => showInForeground;


		public bool DefaultEnabledNotifications => defaultEnabledNotifications;


		public bool AutoSendRequest => autoSendRequest;

		#endregion
	}
}
