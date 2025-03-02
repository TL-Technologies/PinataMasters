using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "gold_buster_offer_settings", menuName = "PinataMasters/Offers/IngameOfferGoldBusterSettings")]
    public class IngameOfferGoldBusterSettings : IngameOfferReactiveSettings
    {
        #region Properties

        protected override bool IsOfferAvailable => CoinsBooster.asset.Value.BoosterDurationLeft < float.Epsilon;

        #endregion

    }
}
