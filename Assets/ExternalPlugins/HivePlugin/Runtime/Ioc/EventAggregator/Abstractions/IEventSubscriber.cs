using System.Threading.Tasks;


namespace Modules.Hive.Ioc
{
    public interface IEventSubscriber<in T> where T : IEvent
    {
        Task OnHandleEventAsync(T evt);
    }
}
