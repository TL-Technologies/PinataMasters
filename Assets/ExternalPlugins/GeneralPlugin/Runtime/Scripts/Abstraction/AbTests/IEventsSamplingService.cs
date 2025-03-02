using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IEventsSamplingService
    {
        bool IsBlockingEventsEnabledForModule(string moduleName);
        List<string> FindBlockedEventsListForModule(string moduleName);
    }
}
