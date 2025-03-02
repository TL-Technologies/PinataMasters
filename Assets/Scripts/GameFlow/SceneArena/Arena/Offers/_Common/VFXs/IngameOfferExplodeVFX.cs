using Modules.General;
using System;
using System.Collections;
using UnityEngine;

namespace PinataMasters
{
    public class IngameOfferExplodeVFX : IngameOfferVFX
    {
        #region Fields

        const float ExplodePositionOffsetY = -1.5f;
        const float ExplodeEffectDelay = 0.33f;

        [SerializeField] Effect explodeVFX = null;
        [SerializeField] IngameOfferBirdAnimation ingameOfferAnimation;

        ParticleEffect vfx;

        #endregion



        #region Public methods

        public override void Spawn(Action callback = null)
        {
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                EffectPlayer.Play(explodeVFX, new Vector3(transform.position.x, transform.position.y + ExplodePositionOffsetY, transform.position.z), transform);
            }, ExplodeEffectDelay);

            ingameOfferAnimation.ApplyUnboxingAnimation(() =>
            {           
                callback?.Invoke();
            });
        }

        #endregion
    }
}
