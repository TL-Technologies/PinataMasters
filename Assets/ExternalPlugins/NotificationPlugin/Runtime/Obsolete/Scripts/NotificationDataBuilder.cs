using System;


namespace Modules.Notification.Obsolete
{
    public class NotificationDataBuilder
    {
        private readonly NotificationData data = new NotificationData()
        {
            DaysRepeat = 1
        };

        

        #region Methods
        
        public NotificationData Build()
        {
            if (data.Key == null)
            {
                throw new ArgumentNullException(nameof(data.Key));
            }

            if (data.Id == null)
            {
                throw new ArgumentNullException(nameof(data.Id));
            }

            if (data.Title == null)
            {
                throw new ArgumentNullException(nameof(data.Title));
            }

            if (data.FireDate == null)
            {
                throw new ArgumentNullException(nameof(data.FireDate));
            }

            return new NotificationData(data);
        }


        public NotificationDataBuilder SetKey(string key)
        {
            data.Key = key;
            return this;
        }


        public NotificationDataBuilder SetId(string id)
        {
            data.Id = id;
            return this;
        }


        public NotificationDataBuilder SetTitle(string title)
        {
            data.Title = title;
            return this;
        }


        public NotificationDataBuilder SetDescription(string description)
        {
            data.Description = description;
            return this;
        }


        public NotificationDataBuilder SetFireDate(DateTime fireDate)
        {
            data.FireDate = fireDate;
            return this;
        }


        public NotificationDataBuilder SetDaysRepeat(int daysRepeat)
        {
            data.DaysRepeat = daysRepeat;
            return this;
        }


        #if UNITY_ANDROID

        public NotificationDataBuilder SetCustomViewFactoryClassName(string className)
        {
            data.CustomViewFactoryClassName = className;
            return this;
        }

        #endif
        
        #endregion
    }
}
