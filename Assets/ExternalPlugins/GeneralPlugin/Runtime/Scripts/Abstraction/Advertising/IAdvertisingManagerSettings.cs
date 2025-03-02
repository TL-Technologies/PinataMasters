using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAdvertisingManagerSettings
    {
        IAdvertisingService[] AdServices { get; set; }
        
        IAdvertisingNecessaryInfo AdvertisingInfo { get; set; }
        
        IAdvertisingAbTestData[] AbTestData { get; set; }
        
        List<AdAvailabilityParameter> AdAvailabilityParameters { get; set; }
        
        List<AdPlacementModel> AdvertisingPlacements { get; set; }
    }
}
