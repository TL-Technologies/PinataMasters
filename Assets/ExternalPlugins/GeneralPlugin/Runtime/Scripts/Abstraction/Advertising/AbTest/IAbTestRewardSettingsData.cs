using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAbTestRewardSettingsData : IAdvertisingAbTestData
    {
        // The implementation of the interface will be serialized.
        // The method is used to avoid duplication of data.
        List<RvPlacementAbTestData> GetRewardsSettings();
    }
}

