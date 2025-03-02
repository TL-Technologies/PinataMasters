using UnityEngine;
using Sirenix.OdinInspector;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "Ingame_offers_settings", menuName = "PinataMasters/Offers/IngameOffersSettings")]
    public class IngameOffersSettings : SerializedScriptableObject
    {
        #region Fields

        [Header("Ingame Offers")]
        [SerializeField] IngameOfferSettings[] ingameOffers;
        [SerializeField] IngameOfferAnimationSettings animationSettings;

        #endregion



        #region Variables

        int? levelForUnlockOffers = null;
        float? negativeCooldown = null;
        float? positiveCooldown = null;
        float? delayBeforeSpawn = null;
        float? lifetime = null;

        #endregion



        #region Properties

        public int LevelForUnlockOffers => (levelForUnlockOffers.HasValue) ? (levelForUnlockOffers.Value) :
            (ABTest.InGameAbTestData.boosterUnlockLevel);


        public float PositiveCooldown => (positiveCooldown.HasValue) ? (positiveCooldown.Value) :
            (Mathf.Max(0.0f, ABTest.InGameAbTestData.boosterCooldownPositive));


        public float NegativeCooldown => (negativeCooldown.HasValue) ? (negativeCooldown.Value) :
            (Mathf.Max(0.0f, ABTest.InGameAbTestData.boosterCooldownNegative));


        public float DelayBeforeSpawn => (delayBeforeSpawn.HasValue) ? (delayBeforeSpawn.Value) :
            (ABTest.InGameAbTestData.boosterSpawnDelay);


        public float Lifetime => (lifetime.HasValue) ? (lifetime.Value) :
            (ABTest.InGameAbTestData.boosterLifetime);


        public IngameOfferSettings[] IngameOffers => ingameOffers;


        public IngameOfferAnimationSettings AnimationSettings => animationSettings;

        #endregion



        #region Debug methods

        public void SetDebugLevelForUnlockOffers(int level)
        {
            levelForUnlockOffers = level;
        }


        public void SetDebugOfferDelayBeforeSpawn(float delay)
        {
            delayBeforeSpawn = delay;
        }


        public void SetDebugLifetime(float time)
        {
            lifetime = time;
        }


        public void SetDebugOfferPositiveCooldown(float cooldown)
        {
            positiveCooldown = cooldown;
        }


        public void SetDebugOfferNegativeCooldown(float cooldown)
        {
            negativeCooldown = cooldown;
        }

        #endregion
    }
}
