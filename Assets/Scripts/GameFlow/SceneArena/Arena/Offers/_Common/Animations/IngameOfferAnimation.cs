using UnityEngine;
using System;


namespace PinataMasters
{
    public abstract class IngameOfferAnimation : MonoBehaviour
    {
        #region Nested types

        public enum Type
        {
            None = 0,
            Spawn = 1,
            Remove = 2,
            Idle = 3
        }

        #endregion



        #region Public methods

        public abstract void Initialize(IngameOfferAnimationSettings settings, IngameOfferHandler offerHandler);

        public abstract void ApplyAnimation(Type animationType, Action callback = null);

        #endregion
    }
}
