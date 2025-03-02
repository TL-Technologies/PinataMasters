using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public class ModulesSamplingAbTestData : IAbTestData
    {
        public virtual bool IsSamplingMigrationAllowed { get; set; } = false;

        public virtual Dictionary<string, SamplingAbTestData> SamplingTestDataPerModule { get; set; } = new Dictionary<string, SamplingAbTestData>
        {
            {"Modules.AppsFlyer.AppsFlyerAnalyticsServiceImplementor", new SamplingAbTestData()},
            {"Modules.Firebase.FirebaseAnalyticsServiceImplementor", new SamplingAbTestData()
            {
                SamplingDropPercent = 0,
                SamplingDropEvents = new List<string>()
            }}
        };
    }
}
