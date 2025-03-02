using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdPlacementsController
    {
        #region Fields

        private readonly Dictionary<AdModule, Dictionary<string, AdPlacementSpec>> advertisingPlacements = 
            new Dictionary<AdModule, Dictionary<string, AdPlacementSpec>>();

        #endregion
        
        

        #region Methods

        public void AddPlacement(AdPlacementSpec placementSpec)
        {
            if (advertisingPlacements.TryGetValue(placementSpec.PlacementModule, out var advertisingPlacementsByModule))
            {
                if (advertisingPlacementsByModule.TryGetValue(placementSpec.PlacementName,
                    out var placementAvailabilityChecker))
                {
                    advertisingPlacementsByModule[placementSpec.PlacementName] = placementSpec;
                }
                else
                {
                    advertisingPlacementsByModule.Add(placementSpec.PlacementName, placementSpec);
                }
            }
            else
            {
                advertisingPlacements.Add(placementSpec.PlacementModule, 
                    new Dictionary<string, AdPlacementSpec>()
                    {
                        { placementSpec.PlacementName, placementSpec }
                    });
            }
        }


        public AdPlacementSpec GetPlacement(AdModule adModule, string placementName)
        {
            if (advertisingPlacements.TryGetValue(adModule,
                out var advertisingPlacementsByModule))
            {
                if (advertisingPlacementsByModule.TryGetValue(placementName, out var adPlacement))
                {
                    return adPlacement;
                }
            }

            return null;
        }


        public bool IsAdModuleByPlacementAvailable(AdModule adModule, string placementName)
        {
            if (advertisingPlacements.TryGetValue(adModule, 
                out var advertisingPlacementsByModule))
            {
                if (advertisingPlacementsByModule.TryGetValue(placementName, out var adPlacement))
                {
                    return adPlacement.IsAvailable;
                }

                if (adModule == AdModule.Interstitial)
                {
                    // All other adModules often use default configuration.
                    CustomDebug.LogWarning($"Can't find {adModule} placement for {placementName}. " +
                                           $"Default placements configuration will be used");
                }

                if (advertisingPlacementsByModule.TryGetValue(AdPlacementType.DefaultPlacement,
                        out adPlacement))
                {
                        AdPlacementSpec newAdPlacement = new AdPlacementSpec(placementName, adPlacement);
                        advertisingPlacementsByModule.Add(placementName, newAdPlacement);
                        
                        return newAdPlacement.IsAvailable;
                }

                CustomDebug.LogError($"Can't find {adModule} placement for {AdPlacementType.DefaultPlacement}");

                return false;
            }

            CustomDebug.LogError($"Can't find {adModule} placement for {placementName}");

            return false;
        }

        #endregion
    }
}
