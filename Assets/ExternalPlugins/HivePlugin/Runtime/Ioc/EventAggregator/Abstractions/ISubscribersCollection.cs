using System;
using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public interface ISubscribersCollection
    {
        int SubscribersCount { get; }

        ISubscriptionToken AddSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent;
        ISubscriptionToken AddSubscriber<T>(Func<T, Task> subscriber) where T : IEvent;
        ISubscriptionToken AddSubscriber(object subscriber);
        bool RemoveSubscriber(ISubscriptionToken token);
        bool RemoveSubscriber<T>(IEventSubscriber<T> subscriber) where T : IEvent;
        bool RemoveSubscriber<T>(Func<T, Task> subscriber) where T : IEvent;
        bool RemoveSubscriber(object subscriber);
    }
}
