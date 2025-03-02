namespace Modules.General.Abstraction
{
    public interface IAnalyticsManagerSettings
    {
        IAnalyticsService[] Services { get; }
    }
}
