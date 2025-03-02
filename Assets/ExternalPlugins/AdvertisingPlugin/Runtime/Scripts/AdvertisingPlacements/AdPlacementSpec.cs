using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdPlacementSpec
    {
        #region Properties

        public AdModule PlacementModule { get; }


        public string PlacementName { get; }


        public bool IsAvailable
        {
            get
            {
                bool result = true;

                foreach (AdAvailabilityParameter adAvailabilityParameter in AdAvailabilityParameters)
                {
                    result &= adAvailabilityParameter.IsAvailable(PlacementName);
                    if (!result)
                    {
                        break;
                    }
                }
                
                if (result && PlacementAvailabilityChecker != null)
                {
                    result &= PlacementAvailabilityChecker.Invoke(PlacementName);
                }
                
                return result;
            }
        }


        private List<AdAvailabilityParameter> AdAvailabilityParameters { get; } = new List<AdAvailabilityParameter>();


        private Func<string, bool> PlacementAvailabilityChecker { get; }

        #endregion
        
        
        
        #region Class lifecycle

        public AdPlacementSpec(
            AdModule placementModule, 
            string placementName,
            List<AdAvailabilityParameter> availabilityParameters = null,
            Func<string, bool> placementAvailabilityChecker = null)
        {
            PlacementModule = placementModule;
            PlacementName = placementName;
            PlacementAvailabilityChecker = placementAvailabilityChecker;

            if (availabilityParameters != null)
            {
                AdAvailabilityParameters.AddRange(availabilityParameters);
                SortAdAvailabilityParameters();
            }
        }


        public AdPlacementSpec(
            string placementName,
            AdPlacementSpec adPlacementSpec,
            List<AdAvailabilityParameter> availabilityParameters = null,
            Func<string, bool> placementAvailabilityChecker = null)
        {
            PlacementName = placementName;
            PlacementModule = adPlacementSpec.PlacementModule;
            PlacementAvailabilityChecker = placement =>
            {
                bool result = true;
        
                if (placementAvailabilityChecker != null)
                {
                    result &= placementAvailabilityChecker.Invoke(placement);
                }

                if (adPlacementSpec.PlacementAvailabilityChecker != null)
                {
                    result &= adPlacementSpec.PlacementAvailabilityChecker.Invoke(placement);
                }
                
                return result;
            };
            
            if (availabilityParameters != null)
            {
                AdAvailabilityParameters.AddRange(availabilityParameters);
            }
            
            AdAvailabilityParameters.AddRange(adPlacementSpec.AdAvailabilityParameters);
            SortAdAvailabilityParameters();
        }

        #endregion



        #region Methods

        private void SortAdAvailabilityParameters()
        {
            AdAvailabilityParameters.Sort(
                (x, y) => x.Priority.CompareTo(y.Priority));
        }

        #endregion
    }
}
