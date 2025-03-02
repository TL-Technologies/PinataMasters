using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAnalyticsManager : IEventLogger, ITypedEventLogger, IMultipleTypedEventLogger,
        IUserPropertySetter, ITypedUserPropertySetter, IMultipleTypedUserPropertySetter
    {
    }
}