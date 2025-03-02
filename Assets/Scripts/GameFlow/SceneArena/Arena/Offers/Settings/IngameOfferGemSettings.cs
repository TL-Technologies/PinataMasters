using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "gold_buster_offer_settings", menuName = "PinataMasters/Offers/IngameOfferGoldBusterSettings")]
    public class IngameOfferGemSettings : IngameOfferReactiveSettings
    {
        #region Properties

        protected override bool IsOfferAvailable => Player.PresigeLevel > 0;

        #endregion

    }
}
