using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    internal class SimpleSubscriberDescriptor<TEvent> : ISingleEventSubscriberDescriptor<TEvent>, ISubscriptionToken where TEvent : IEvent
    {
        private readonly IEventSubscriber<TEvent> subscriber;

        
        public SimpleSubscriberDescriptor(IEventSubscriber<TEvent> subscriber)
        {
            this.subscriber = subscriber;
        }
        

        public bool HandlesEvent<T>() where T : IEvent
        {
            return typeof(TEvent) == typeof(T);
        }
        

        public Task PublishEventAsync(TEvent evt)
        {
            return subscriber.OnHandleEventAsync(evt);
        }
        

        public override bool Equals(object obj)
        {
            return obj is SimpleSubscriberDescriptor<TEvent> sd && Equals(sd.subscriber, subscriber);
        }
        

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + subscriber.GetHashCode();
                hash = hash * 31 + typeof(TEvent).GetHashCode();
                return hash;
            }
        }
    }
}
