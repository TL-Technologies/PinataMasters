using System;
using System.Threading.Tasks;


namespace Modules.General.Utilities
{
    public interface IAdvertisingIdentifier
    {
        void GetAdvertisingIdentifier(Action<string> callback, bool shouldResetCachedId = false);
        Task<string> GetAdvertisingIdentifierAsync(int delay = 250, bool shouldResetCachedId = false);
    }
}
