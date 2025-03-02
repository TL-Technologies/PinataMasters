using Modules.General.Abstraction;
using System;
using System.Collections.Generic;


namespace Modules.Advertising
{
    public class AdsRewardedVideoPlacementsController
    {
        #region Fields

        private Dictionary<string, AbTestRvSettings> remoteRewardsSettings = null;

        #endregion



        #region Properties

        public static AbTestRvSettings DefaultRewardSettings => new AbTestRvSettings
        {
            RewardedMain = new[]
            {
                RewardedVideoShowingAdsState.None, 
                RewardedVideoShowingAdsState.FreeReward,
                RewardedVideoShowingAdsState.None,
                RewardedVideoShowingAdsState.Rewarded
            },
            RewardedLoop = new[]
            {
                RewardedVideoShowingAdsState.Rewarded, 
                RewardedVideoShowingAdsState.None,
            },
            RewardValue = 50.0f
        };

        #endregion



        #region Class lifecycle

        public AdsRewardedVideoPlacementsController(Dictionary<string, AbTestRvSettings> abTestRewardSettingsData)
        {
            remoteRewardsSettings = abTestRewardSettingsData;
        }

        #endregion



        #region Methods

        public RewardedVideoPlacementSettings GetPlacementSettings(string placement, int level = 0)
        {
            return (IsValidPlacement(placement)) ? (GetPlacementSettingsResult(placement, level)) : 
                (new RewardedVideoPlacementSettings());
        }


        private bool IsValidPlacement(string placement)
        {
            bool isValid = remoteRewardsSettings.ContainsKey(placement);

            if (!isValid)
            {
                throw new Exception($"Placement {placement} is not valid");
            }

            return isValid;
        }


        private RewardedVideoPlacementSettings GetPlacementSettingsResult(string placement, int level = 0)
        {
            RewardedVideoPlacementSettings result = new RewardedVideoPlacementSettings();

            if (remoteRewardsSettings.TryGetValue(placement, out AbTestRvSettings rewardSettings))
            {
                if (level < 1)
                {
                    throw new Exception($"Not valid level for {placement}");
                }
                else
                {
                    RewardedVideoShowingAdsState adsState = 0;

                    if (level <= rewardSettings.RewardedMain.Length)
                    {
                        adsState = rewardSettings.RewardedMain[level - 1];
                    }
                    else if (rewardSettings.RewardedLoop.Length > 0)
                    {
                        adsState = rewardSettings.RewardedLoop[
                            (level - (rewardSettings.RewardedMain.Length + 1)) % rewardSettings.RewardedLoop.Length];
                    }

                    result.showAdsState = adsState;
                    result.reward = rewardSettings.RewardValue;
                }
            }
            else
            {
                throw new Exception($"No RewardSettings for {placement}");
            }

            return result;
        }

        #endregion
    }
}

