using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IPurchaseAnalyticsParameters
    {
        Dictionary<string, Func<string>> Parameters { get; }

        
        void SetParameter(string parameterName, Func<string> parameterRetrievalFunction);
    }
}
