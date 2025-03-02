using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "offer_settings", menuName = "PinataMasters/Offers/Rewards/IngameOfferReward")]
    public class IngameOfferRewardSettings : ScriptableObject
    {
        #region Fields

        [SerializeField] float value = 0.0f;
        [SerializeField] IngameOfferType offerType;

        #endregion



        #region Properties

        public float Value
        {
            get
            {
                if (offerType == IngameOfferType.CoinBonus)
                {
                    float subscriptionMultiplier = (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive) ? (2.0f) : (1.0f);

                    return Mathf.Round(ShooterShadowsConfig.GetAllSkinsWeaponsPower() *
                                       subscriptionMultiplier *
                                       CoinsBooster.asset.Value.CoinsMultiplier *
                                       Player.GetBonusCoinsSkill() *
                                       ABTest.InGameAbTestData.videoShopItemRewardMultiplier);
                }
                else if (offerType == IngameOfferType.X2CoinBonus)
                {
                    float subscriptionMultiplier = (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive) ? (2.0f) : (1.0f);

                    return Mathf.Round(SelectorLevels.GetLevels.GetHealthPinata(Player.Level) *
                                       Value *
                                       subscriptionMultiplier *
                                       CoinsBooster.asset.Value.CoinsMultiplier *
                                       Player.GetBonusCoinsSkill());
                }
                else
                {
                    return value;
                }
            }
        }


        public IngameOfferType OfferType
        {
            get
            {
                return offerType;
            }
        }

        #endregion
    }
}
