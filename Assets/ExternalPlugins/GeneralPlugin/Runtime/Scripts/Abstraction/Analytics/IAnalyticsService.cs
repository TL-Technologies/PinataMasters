using Modules.General.ServicesInitialization;


namespace Modules.General.Abstraction
{
    public interface IAnalyticsService : IInitializable, IEventLogger, IUserPropertySetter
    {
        bool IsAsyncWorkAvailable { get; }

        void SetDeviceId(string userId);
    }
}