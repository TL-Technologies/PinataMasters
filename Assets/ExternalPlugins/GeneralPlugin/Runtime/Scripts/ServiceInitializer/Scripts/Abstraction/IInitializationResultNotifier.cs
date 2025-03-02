using Modules.General.ServicesInitialization;
using System;


namespace Modules.General.Abstraction
{
    public interface IInitializationResultNotifier
    {
        event Action<InitializationStatus> OnInitialized;
    }
}
