using Modules.General.Abstraction;


namespace Modules.Analytics
{
    public class AnalyticsManagerSettings : IAnalyticsManagerSettings
    {
        #region Properties

        public IAnalyticsService[] Services { get; set; }

        #endregion
    }
}
