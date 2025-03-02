using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.Analytics
{
    public class PurchaseAnalyticsParametersImplementor : IPurchaseAnalyticsParameters
    {
        #region Class lifecycle
        
        public PurchaseAnalyticsParametersImplementor() { }
        
        
        public PurchaseAnalyticsParametersImplementor(Dictionary<string, Func<string>> preDefinedParameters)
        {
            Parameters = preDefinedParameters;
        }
        
        #endregion
        
        
        
        #region IPurchaseAnalyticsParameters
        
        public Dictionary<string, Func<string>> Parameters { get; } = new Dictionary<string, Func<string>>();


        public void SetParameter(string parameterName, Func<string> parameterRetrievalFunction)
        {
            if (Parameters.ContainsKey(parameterName))
            {
                Parameters[parameterName] = parameterRetrievalFunction;
            }
            else
            {
                Parameters.Add(parameterName, parameterRetrievalFunction);
            }
        }
        
        #endregion
    }
}
