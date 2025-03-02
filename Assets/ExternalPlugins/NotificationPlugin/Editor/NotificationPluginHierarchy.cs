using Modules.Hive.Editor;


namespace Modules.Notification.Editor
{
    public class NotificationPluginHierarchy : PluginHierarchy
    {
        private static NotificationPluginHierarchy instance;

        public static NotificationPluginHierarchy Instance =>
            instance ?? (instance = new NotificationPluginHierarchy("Modules.Notification"));

        private NotificationPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }
    }
}
