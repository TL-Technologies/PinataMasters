using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.InAppPurchase
{
    public class PurchaseAnalyticsParametersDummy : IPurchaseAnalyticsParameters
    {
        public Dictionary<string, Func<string>> Parameters { get; } = new Dictionary<string, Func<string>>();


        public void SetParameter(string parameterName, Func<string> parameterRetrievalFunction) { }
    }
}
