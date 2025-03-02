using UnityEngine;
using System;


namespace PinataMasters
{
    public abstract class IngameOfferReward : MonoBehaviour
    {
        #region Nested types

        public struct ClaimResult
        {
            public bool animated;
            public bool claimed;
        }

        #endregion



        #region Fields

        [SerializeField] protected float rewardDelay = 0.0f;

        #endregion



        #region Properties

        protected IngameOfferRewardSettings RewardSettings { get; private set; }

        #endregion



        #region Public methods

        public virtual void Initialize(IngameOfferRewardSettings rewardSettings)
        {
            RewardSettings = rewardSettings;
        }

        public abstract void TryClaimReward(Action<ClaimResult> callback);

        #endregion
    }
}
