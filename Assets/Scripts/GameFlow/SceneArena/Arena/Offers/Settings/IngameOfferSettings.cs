using UnityEngine;
using Sirenix.OdinInspector;


namespace PinataMasters
{
    public abstract class IngameOfferSettings : SerializedScriptableObject
    {
        #region Fields

        [SerializeField] IngameOffer ingameOffer;
        [SerializeField] IngameOfferRewardSettings rewardSettings;

        #endregion



        #region Properties

        public IngameOffer IngameOffer => ingameOffer;

        public IngameOfferRewardSettings RewardSettings => rewardSettings;

        public abstract float Weight { get; }

        #endregion



        #region Public methods

        public abstract void ApplyReaction(bool positive);

        #endregion
    }
}
