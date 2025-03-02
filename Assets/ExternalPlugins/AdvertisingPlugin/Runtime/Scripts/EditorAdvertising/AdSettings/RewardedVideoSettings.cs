using Modules.General.Abstraction;
using System;


namespace Modules.Advertising
{
    [Serializable]
    public class RewardedVideoSettings : AdModuleSettings
    {
        #region Properties

        public override AdModule ModuleType => AdModule.RewardedVideo;

        #endregion
    }
}