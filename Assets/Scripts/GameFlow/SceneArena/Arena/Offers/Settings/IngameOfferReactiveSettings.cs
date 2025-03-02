using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "reactive_offer_settings", menuName = "PinataMasters/Offers/IngameOfferRelativeSettings")]
    public class IngameOfferReactiveSettings : IngameOfferSettings
    {
        #region Fields

        [SerializeField] float weight;
        [SerializeField] float[] negativeWeights;

        float? actualWeight;

        int negativeReactionsCount = 0;

        bool isCurrentWeightPositive = true;

        #endregion



        #region Properties

        public override float Weight => IsOfferAvailable ? ActualWeight : 0f;


        protected float ActualWeight
        {
            get
            {
                return (actualWeight ?? (actualWeight = weight)).Value;
            }
            set
            {
                actualWeight = value;
            }
        }


        protected virtual bool IsOfferAvailable => true;

        #endregion



        #region Public methods

        public override void ApplyReaction(bool isPositive)
        {
            if (isPositive || negativeWeights.Length == 0)
            {
                ActualWeight = weight;
                negativeReactionsCount = 0;
            }
            else
            {
                ActualWeight = negativeWeights[negativeReactionsCount];

                if (!isCurrentWeightPositive)
                {
                    negativeReactionsCount = Mathf.Min(negativeReactionsCount + 1, negativeWeights.Length - 1);
                }
            }

            isCurrentWeightPositive = isPositive;
        }

        #endregion
    }
}
