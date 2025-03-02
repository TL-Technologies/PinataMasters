namespace Modules.General.Abstraction
{
    public class AbTestRvSettings
    {
        #region Properties

        public RewardedVideoShowingAdsState[] RewardedMain { get; set; }
            = new[]
            {
                RewardedVideoShowingAdsState.None,
                RewardedVideoShowingAdsState.FreeReward,
                RewardedVideoShowingAdsState.None,
                RewardedVideoShowingAdsState.Rewarded
            };

        public RewardedVideoShowingAdsState[] RewardedLoop { get; set; }
            = new[]
            {
                RewardedVideoShowingAdsState.Rewarded,
                RewardedVideoShowingAdsState.None,
            };

        public float RewardValue { get; set; } = 50.0f;

        #endregion
    }
}
