using Modules.General.Abstraction;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdAvailabilityParametersLib
    {
        #region Fields

        private readonly Dictionary<AdModule, Dictionary<string, AdAvailabilityParameter>> adParameters = 
            new Dictionary<AdModule, Dictionary<string, AdAvailabilityParameter>>();

        #endregion



        #region Methods

        public void Add(AdAvailabilityParameter availabilityParameter)
        {
            if (adParameters.TryGetValue(availabilityParameter.ParameterModule, out var adAvailabilityParameters))
            {
                if (adAvailabilityParameters.TryGetValue(availabilityParameter.ParameterName,
                    out var adAvailabilityParameter))
                {
                    CustomDebug.LogWarning($"Update {availabilityParameter.ParameterModule} " +
                                           $"placement {availabilityParameter.ParameterName} availability");
                    
                    adAvailabilityParameters[availabilityParameter.ParameterName] = availabilityParameter;
                }
                else
                {
                    adAvailabilityParameters.Add(availabilityParameter.ParameterName, availabilityParameter);
                }
            }
            else
            {
                adParameters.Add(availabilityParameter.ParameterModule, 
                    new Dictionary<string, AdAvailabilityParameter>()
                    {
                        { availabilityParameter.ParameterName, availabilityParameter }
                    });
            }
        }


        public void Add(AdAvailabilityParameter[] availabilityParameters)
        {
            foreach (AdAvailabilityParameter adAvailabilityParameter in availabilityParameters)
            {
                Add(adAvailabilityParameter);
            }
        }


        public AdAvailabilityParameter GetAdAvailabilityParameter(AdModule adModule, string key)
        {
            Dictionary<string, AdAvailabilityParameter> availabilityParameters = adParameters[adModule];
            return availabilityParameters?[key];
        }

        #endregion
    }
}
