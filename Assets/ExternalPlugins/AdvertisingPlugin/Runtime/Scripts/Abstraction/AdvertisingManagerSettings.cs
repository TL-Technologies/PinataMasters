using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdvertisingManagerSettings : IAdvertisingManagerSettings
    {
        public IAdvertisingService[] AdServices { get; set; }

        public IAdvertisingNecessaryInfo AdvertisingInfo { get; set; }

        public IAdvertisingAbTestData[] AbTestData { get; set; }
        
        public List<AdAvailabilityParameter> AdAvailabilityParameters { get; set; }

        public List<AdPlacementModel> AdvertisingPlacements { get; set; }
    }
}
