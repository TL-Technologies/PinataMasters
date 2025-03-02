using Modules.Hive.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public partial class EventAggregator : IEventAggregator
    {
        #region Fields
        
        private HashSet<ISubscriberDescriptor> subscribers = new HashSet<ISubscriberDescriptor>();
        
        #endregion
        
        
        
        #region Main methods
        
        public int SubscribersCount => subscribers.Count;

        
        public bool RemoveSubscriber(ISubscriptionToken token)
        {
            return token is ISubscriberDescriptor sd && RemoveSubscriberImplementation(sd);
        }

        
        public IEventAggregatorScope CreateScope()
        {
            return new Scope(this);
        }
        

        private void AddSubscriberImplementation(ISubscriberDescriptor subscriberDescriptor)
        {
            lock (subscribers)
            {
                subscribers.Add(subscriberDescriptor);
            }
        }
        

        private bool RemoveSubscriberImplementation(ISubscriberDescriptor subscriberDescriptor)
        {
            lock (subscribers)
            {
                return subscribers.Remove(subscriberDescriptor);
            }
        }
        

        private void RemoveSubscribersImplementation(IEnumerable<ISubscriptionToken> tokens)
        {
            lock (subscribers)
            {
                foreach (var token in tokens)
                {
                    if (token is ISubscriberDescriptor sd)
                    {
                        subscribers.Remove(sd);
                    }
                }
            }
        }

        #endregion
        
        
        
        #region Single event subscriber

        public ISubscriptionToken AddSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new SimpleSubscriberDescriptor<T>(subscriber);
            AddSubscriberImplementation(sd);
            return sd;
        }

        
        public bool RemoveSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return false;
            }

            var sd = new SimpleSubscriberDescriptor<T>(subscriber);
            return RemoveSubscriberImplementation(sd);
        }

        
        private ISubscriptionToken RemoveSubscriberInternal<T>(IEventSubscriber<T> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new SimpleSubscriberDescriptor<T>(subscriber);
            return RemoveSubscriberImplementation(sd) ? sd : null;
        }

        #endregion
        
        

        #region Multi-event subscriber

        public ISubscriptionToken AddSubscriber(object subscriber)
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new ComplexSubscriberDescriptor(subscriber);
            AddSubscriberImplementation(sd);
            return sd;
        }

        
        public bool RemoveSubscriber(object subscriber)
        {
            if (subscriber == null)
            {
                return false;
            }

            var sd = new ComplexSubscriberDescriptor(subscriber);
            return RemoveSubscriberImplementation(sd);
        }
        

        private ISubscriptionToken RemoveSubscriberInternal(object subscriber)
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new ComplexSubscriberDescriptor(subscriber);
            return RemoveSubscriberImplementation(sd) ? sd : null;
        }

        #endregion
        
        

        #region Delegate subscriber

        public ISubscriptionToken AddSubscriber<T>(Func<T, Task> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new DelegateSubscriberDescriptor<T>(subscriber);
            AddSubscriberImplementation(sd);
            return sd;
        }
        

        public bool RemoveSubscriber<T>(Func<T, Task> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return false;
            }

            var sd = new DelegateSubscriberDescriptor<T>(subscriber);
            return RemoveSubscriberImplementation(sd);
        }
        

        private ISubscriptionToken RemoveSubscriberInternal<T>(Func<T, Task> subscriber) where T : IEvent
        {
            if (subscriber == null)
            {
                return null;
            }

            var sd = new DelegateSubscriberDescriptor<T>(subscriber);
            return RemoveSubscriberImplementation(sd) ? sd : null;
        }

        #endregion

        
        
        #region Event publishing

        public Task PublishEventAsync<T>(T evt) where T : IEvent
        {
            if (evt == null)
            {
                return Task.CompletedTask;
            }

            // Make copy for threadsafety
            List<Task> tasks = null;
            lock (subscribers)
            {
                // Handlers retrieving and event publishing should be separated to avoid problems
                // in subscribing or unsubscribing during the event publishing process
                List<ISubscriberDescriptor> handlers = subscribers
                    .Where(s => s.HandlesEvent<T>())
                    .ToList();
                tasks = handlers
                    .Select(h => PublishEventAsync(h, evt))
                    .Where(p => p != null) // To avoid potential exceptions
                    .ToList();
            }

            return Task.WhenAll(tasks);
        }
        

        private static Task PublishEventAsync<T>(ISubscriberDescriptor subscriberDescriptor, T evt) where T : IEvent
        {
            Task task = null;

            switch (subscriberDescriptor)
            {
                case ISingleEventSubscriberDescriptor<T> sed:
                    task = sed.PublishEventAsync(evt);
                    break;
                case IMultiEventsSubscriberDescriptor med:
                    task = med.PublishEventAsync(evt);
                    break;
                default:
                    Assert.Throw($"Failed to publish event of type '{typeof(T)}' to subscriber that represented by descriptor of unknown type '{subscriberDescriptor?.GetType()}'");
                    task = null;
                    break;
            }

            return task;
        }

        #endregion
        
        

        #region Testing

        internal ICollection<ISubscriberDescriptor> GetSubscriberDescriptors()
        {
            return subscribers;
        }

        #endregion
    }
}
