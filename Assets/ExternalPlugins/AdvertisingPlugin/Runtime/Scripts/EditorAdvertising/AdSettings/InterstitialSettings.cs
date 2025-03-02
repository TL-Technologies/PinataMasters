using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    [Serializable]
    public class InterstitialSettings : AdModuleSettings
    {
        #region Properties

        public override AdModule ModuleType => AdModule.Interstitial;

        #endregion
    }
}