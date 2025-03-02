using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    internal class ComplexSubscriberDescriptor : IMultiEventsSubscriberDescriptor, ISubscriptionToken
    {
        private readonly object subscriber;

        
        public ComplexSubscriberDescriptor(object subscriber)
        {
            this.subscriber = subscriber;
        }
        

        public bool HandlesEvent<T>() where T : IEvent
        {
            return subscriber is IEventSubscriber<T>;
        }
        

        public Task PublishEventAsync<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var sb = subscriber as IEventSubscriber<TEvent>;
            if (sb == null)
            {
                UnityEngine.Debug.LogWarning($"Failed to pass an event of type '{typeof(TEvent)}' to subscriber of type '{subscriber.GetType()}' because the subscriber doesn't inherit interface {nameof(IEventSubscriber<TEvent>)}");
                return null;
            }

            return sb.OnHandleEventAsync(evt);
        }
        

        public override bool Equals(object obj)
        {
            return obj is ComplexSubscriberDescriptor sd && Equals(sd.subscriber, subscriber);
        }
        

        public override int GetHashCode()
        {
            return subscriber.GetHashCode();
        }
    }
}
