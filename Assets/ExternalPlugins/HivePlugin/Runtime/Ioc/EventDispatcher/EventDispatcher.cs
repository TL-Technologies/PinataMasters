using System;
using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public static class EventDispatcher
    {
        #region Fields

        private static EventAggregator eventAggregator = new EventAggregator();

        #endregion



        #region Methods

        private static ISubscriptionToken Subscribe<T>(Func<T, Task> callback) where T : IEvent
        {
            return eventAggregator.AddSubscriber(callback);
        }


        public static ISubscriptionToken Subscribe<T>(Action<T> callback) where T : IEvent
        {
            Func<T, Task> wrapper = x =>
            {
                callback(x);
                return Task.CompletedTask;
            };

            return Subscribe(wrapper);
        }


        public static void Dispatch<T>(T evt) where T : IEvent
        {
            eventAggregator.PublishEventAsync(evt);
        }


        public static void Unsubscribe(ISubscriptionToken token)
        {
            eventAggregator.RemoveSubscriber(token);
        }

        #endregion
    }
}
