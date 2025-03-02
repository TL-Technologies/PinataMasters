using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "speed_buster_offer_settings", menuName = "PinataMasters/Offers/IngameOfferSpeedBusterSettings")]
    public class IngameOfferSpeedBusterSettings : IngameOfferReactiveSettings
    {
        #region Properties

        protected override bool IsOfferAvailable => ShooterUpgradesBooster.asset.Value.BoosterDurationLeft < float.Epsilon; 

        #endregion

    }
}
