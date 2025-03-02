using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdPlacementBuilder
    {
        #region Fields

        private readonly AdAvailabilityParametersLib availabilityParametersLib;
        private readonly AdPlacementsController placementsController;

        #endregion



        #region Class lifecycle

        public AdPlacementBuilder(
            AdAvailabilityParametersLib availabilityParametersLib,
            AdPlacementsController placementsController)
        {
            this.availabilityParametersLib = availabilityParametersLib;
            this.placementsController = placementsController;
        }

        #endregion



        #region Methods

        public AdPlacementSpec CreatePlacementSpec(AdPlacementModel placementModel)
        {
            AdPlacementSpec result = null;

            List<AdAvailabilityParameter> adAvailabilityParameters = new List<AdAvailabilityParameter>();
            if (placementModel.AvailabilityParametersKeys != null)
            {
                foreach (string parameterKey in placementModel.AvailabilityParametersKeys)
                {
                    adAvailabilityParameters.Add(availabilityParametersLib.GetAdAvailabilityParameter(
                        placementModel.PlacementModule, parameterKey));
                }
            }

            if (!string.IsNullOrEmpty(placementModel.ExistedPlacementName))
            {
                AdPlacementSpec existedPlacement = placementsController.GetPlacement(placementModel.PlacementModule,
                    placementModel.ExistedPlacementName);
                if (existedPlacement == null)
                {
                    throw new ArgumentException(
                        $"{placementModel.PlacementModule} placement with name " +
                        $"{placementModel.PlacementName} doesn't exist");
                }

                result = new AdPlacementSpec(
                    placementModel.PlacementName,
                    existedPlacement,
                    adAvailabilityParameters,
                    placementModel.PlacementAvailabilityChecker);
            }
            else
            {
                result = new AdPlacementSpec(
                    placementModel.PlacementModule,
                    placementModel.PlacementName,
                    adAvailabilityParameters,
                    placementModel.PlacementAvailabilityChecker);
            }

            return result;
        }

        #endregion
    }
}
