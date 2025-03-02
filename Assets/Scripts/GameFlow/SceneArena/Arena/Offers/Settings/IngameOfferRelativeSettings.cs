using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "relative_offer_settings", menuName = "PinataMasters/Offers/IngameOfferRelativeSettings")]
    public class IngameOfferRelativeSettings : IngameOfferSettings
    {
        #region Fields

        [SerializeField] IngameOfferSettings[] settings;

        #endregion



        #region Properties

        public override float Weight
        {
            get
            {
                float weight = 0f;

                for (int idx = 0; idx < settings.Length; idx++)
                {
                    weight += settings[idx].Weight;
                }

                return weight / settings.Length;
            }
        }

        #endregion



        #region Public methods

        public override void ApplyReaction(bool positive)
        {

        }

        #endregion
    }
}
