namespace Modules.General.Abstraction
{
    public interface IAnalyticsProcessor : IInitializationResultNotifier, IEventLogger, ITypedEventLogger, IUserPropertySetter, ITypedUserPropertySetter
    {
        void Initialize(IAnalyticsService[] analyticsServices);

        T GetService<T>() where T : class, IAnalyticsService;
    }
}