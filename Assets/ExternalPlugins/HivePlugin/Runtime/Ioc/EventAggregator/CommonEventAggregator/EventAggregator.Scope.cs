using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public partial class EventAggregator
    {
        private class Scope : IEventAggregatorScope
        {
            private readonly EventAggregator eventAggregator;
            private HashSet<ISubscriptionToken> tokens = new HashSet<ISubscriptionToken>();

            
            public int SubscribersCount => tokens.Count;
            
            
            public Scope(EventAggregator eventAggregator)
            {
                this.eventAggregator = eventAggregator;
            }
            
            
            public bool RemoveSubscriber(ISubscriptionToken token)
            {
                bool rs = eventAggregator.RemoveSubscriber(token);
                if (token != null)
                {
                    lock (tokens)
                    {
                        tokens.Remove(token);
                    }
                }

                return rs;
            }


            private ISubscriptionToken AddToken(ISubscriptionToken token)
            {
                if (token != null)
                {
                    lock (tokens)
                    {
                        tokens.Add(token);
                    }
                }

                return token;
            }

            
            private bool RemoveToken(ISubscriptionToken token)
            {
                if (token != null)
                {
                    lock (tokens)
                    {
                        tokens.Remove(token);
                    }

                    return true;
                }

                return false;
            }


            #region Single event subscriber

            public ISubscriptionToken AddSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent
            {
                ISubscriptionToken token = eventAggregator.AddSubscriber(subscriber);
                return AddToken(token);
            }

            
            public bool RemoveSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent
            {
                ISubscriptionToken token = eventAggregator.RemoveSubscriberInternal(subscriber);
                return RemoveToken(token);
            }

            #endregion
            
            

            #region Multi-event subscriber

            public ISubscriptionToken AddSubscriber(object subscriber)
            {
                ISubscriptionToken token = eventAggregator.AddSubscriber(subscriber);
                return AddToken(token);
            }

            
            public bool RemoveSubscriber(object subscriber)
            {
                ISubscriptionToken token = eventAggregator.RemoveSubscriberInternal(subscriber);
                return RemoveToken(token);
            }

            #endregion
            
            

            #region Delegate subscriber

            public ISubscriptionToken AddSubscriber<T>(Func<T, Task> subscriber) where T : IEvent
            {
                ISubscriptionToken token = eventAggregator.AddSubscriber(subscriber);
                return AddToken(token);
            }

            
            public bool RemoveSubscriber<T>(Func<T, Task> subscriber) where T : IEvent
            {
                ISubscriptionToken token = eventAggregator.RemoveSubscriberInternal(subscriber);
                return RemoveToken(token);
            }

            #endregion

            public void Dispose()
            {
                lock (tokens)
                {
                    if (tokens.Count > 0)
                    {
                        eventAggregator.RemoveSubscribersImplementation(tokens);
                        tokens.Clear();
                    }
                }
            }
        }
    }
}
