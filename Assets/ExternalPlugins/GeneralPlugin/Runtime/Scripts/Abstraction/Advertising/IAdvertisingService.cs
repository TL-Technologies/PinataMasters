using Modules.Advertising;
using Modules.General.ServicesInitialization;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAdvertisingService : IInitializable
    {
        bool IsSupportedByCurrentPlatform { get; }

        string ServiceName { get; }

        List<IEventAds> SupportedAdsModules { get; }
    }
}
