using System;


namespace Modules.General.Abstraction
{
    [Flags]
    public enum AdModule
    {
        None            = 0,
        RewardedVideo   = 1 << 0,
        Interstitial    = 1 << 1,
        Banner          = 1 << 2
    }
}
