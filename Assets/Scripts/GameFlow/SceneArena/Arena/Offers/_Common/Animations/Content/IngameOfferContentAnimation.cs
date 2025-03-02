using UnityEngine;


namespace PinataMasters
{
    public abstract class IngameOfferContentAnimation : MonoBehaviour
    {
        #region Public methods

        public abstract void UpdateContentForDirection(IngameOfferAnimationDirection direction);

        #endregion
    }
}
