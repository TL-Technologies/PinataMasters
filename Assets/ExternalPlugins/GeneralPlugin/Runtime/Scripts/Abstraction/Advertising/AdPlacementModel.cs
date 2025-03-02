using System;


namespace Modules.General.Abstraction
{
    public class AdPlacementModel
    {
        #region Properties

        public AdModule PlacementModule { get; }
        
        public string PlacementName { get; }
        
        public string ExistedPlacementName { get; }

        public string[] AvailabilityParametersKeys { get; }
        
        public Func<string, bool> PlacementAvailabilityChecker { get; }

        #endregion
        
        
        
        #region Class lifecycle

        public AdPlacementModel(
            AdModule placementModule,
            string placementName,
            string[] availabilityParametersKeys = null,
            Func<string, bool> placementAvailabilityChecker = null)
        {
            PlacementModule = placementModule;
            PlacementName = placementName;
            AvailabilityParametersKeys = availabilityParametersKeys;
            PlacementAvailabilityChecker = placementAvailabilityChecker;
        }


        public AdPlacementModel(
            AdModule placementModule,
            string placementName,
            string existedPlacementName = null,
            string[] availabilityParametersKeys = null,
            Func<string, bool> placementAvailabilityChecker = null)
        {
            PlacementModule = placementModule;
            PlacementName = placementName;
            ExistedPlacementName = existedPlacementName;
            AvailabilityParametersKeys = availabilityParametersKeys;
            PlacementAvailabilityChecker = placementAvailabilityChecker;
        }

        #endregion
    }
}
