using System;


namespace Modules.Notification.Obsolete
{
    public class NotificationData
    {
        #region Properties
        
        public string Key { get; internal set; }
        
        public string Id { get; internal set; }
        
        public string Title { get; internal set; }
        
        public string Description { get; internal set; }
        
        public DateTime FireDate { get; internal set; }
        
        public int DaysRepeat { get; internal set; }

        #if UNITY_ANDROID
            public string CustomViewFactoryClassName { get; internal set; }
        #endif
        
        #endregion



        #region Class lifecycle

        internal NotificationData() { }


        internal NotificationData(NotificationData other)
        {
            Key = other.Key;
            Id = other.Id;
            Title = other.Title;
            Description = other.Description;
            FireDate = other.FireDate;
            DaysRepeat = other.DaysRepeat;

            #if UNITY_ANDROID
                CustomViewFactoryClassName = other.CustomViewFactoryClassName;
            #endif
        }
        
        #endregion
    }
}
