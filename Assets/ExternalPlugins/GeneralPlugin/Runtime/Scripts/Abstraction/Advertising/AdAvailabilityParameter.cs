using System;


namespace Modules.General.Abstraction
{
    public class AdAvailabilityParameter
    {
        #region Fields

        private readonly Func<string, bool> availabilityCheckFunc;

        #endregion
        
        
        
        #region Properties

        public AdModule ParameterModule { get; }

        
        public string ParameterName { get; }

        
        public int Priority { get; } = 0;

        #endregion

        

        #region Class lifecycle

        public AdAvailabilityParameter(AdModule parameterModule, string parameterName, Func<string, bool> availabilityFunc)
        {
            ParameterModule = parameterModule;
            ParameterName = parameterName;
            availabilityCheckFunc = availabilityFunc;
        }
        
        
        public AdAvailabilityParameter(
            AdModule parameterModule, 
            string parameterName, 
            Func<string, bool> availabilityFunc, 
            int priority)
        {
            ParameterModule = parameterModule;
            ParameterName = parameterName;
            availabilityCheckFunc = availabilityFunc;
            
            Priority = priority;
        }

        #endregion



        #region Methods

        public bool IsAvailable(string placement) => availabilityCheckFunc.Invoke(placement);

        #endregion
    }
}