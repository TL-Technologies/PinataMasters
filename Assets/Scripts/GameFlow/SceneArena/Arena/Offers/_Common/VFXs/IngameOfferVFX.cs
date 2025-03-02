using UnityEngine;
using System;


namespace PinataMasters
{
    public abstract class IngameOfferVFX : MonoBehaviour
    {
        #region Public methods

        public abstract void Spawn(Action callback = null);

        #endregion
    }
}
