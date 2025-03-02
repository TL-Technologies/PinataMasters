using System;
using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    internal class DelegateSubscriberDescriptor<TEvent> : ISingleEventSubscriberDescriptor<TEvent>, ISubscriptionToken where TEvent : IEvent
    {
        private readonly Func<TEvent, Task> subscriber;

        public DelegateSubscriberDescriptor(Func<TEvent, Task> subscriber)
        {
            this.subscriber = subscriber;
        }

        
        public bool HandlesEvent<T>() where T : IEvent
        {
            return typeof(TEvent) == typeof(T);
        }

        
        public Task PublishEventAsync(TEvent evt)
        {
            return subscriber(evt);
        }

        
        public override bool Equals(object obj)
        {
            var sd = obj as DelegateSubscriberDescriptor<TEvent>;
            return sd != null && sd.subscriber == subscriber;
        }

        
        public override int GetHashCode()
        {
            return subscriber.GetHashCode();
        }
    }
}
