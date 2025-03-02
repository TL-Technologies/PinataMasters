using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    internal interface ISubscriberDescriptor
    {
        bool HandlesEvent<T>() where T : IEvent;
    }
    

    internal interface ISingleEventSubscriberDescriptor<T> : ISubscriberDescriptor where T : IEvent
    {
        Task PublishEventAsync(T evt);
    }
    

    internal interface IMultiEventsSubscriberDescriptor : ISubscriberDescriptor
    {
        Task PublishEventAsync<T>(T evt) where T : IEvent;
    }
}
