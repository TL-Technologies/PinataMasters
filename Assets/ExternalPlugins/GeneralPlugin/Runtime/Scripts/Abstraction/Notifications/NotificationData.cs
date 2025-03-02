using System;


namespace Modules.General.Abstraction
{
    public struct NotificationData
    {
        #region Fields
        
        public string notificationId;
        public string notificationTitle;
        public string notificationText;
        public DateTime fireDate;
        public int notificationShowCount;
        public string intentData;

        #endregion



        #region Class lifecycle

        public NotificationData(string notificationId, string notificationText, DateTime fireDate, 
            int notificationShowCount, string notificationTitle = null, string intentData = null)
        {
            this.notificationId = notificationId;
            this.notificationText = notificationText;
            this.fireDate = fireDate;
            this.notificationShowCount = notificationShowCount;
            this.notificationTitle = notificationTitle;
            this.intentData = intentData;
        }

        #endregion
    }
}
