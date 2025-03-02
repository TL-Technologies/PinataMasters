using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public interface IEventAggregator : ISubscribersCollection
    {
        Task PublishEventAsync<T>(T evt) where T : IEvent;
        IEventAggregatorScope CreateScope();
    }
}
