using UnityEngine;


namespace PinataMasters
{
    public class ObstacleEffect : ParticleEffect
    {
        #region Variables

        [SerializeField]
        private ParticleSystem partsEffect = null;
        [SerializeField]
        private AnimationCurve hitCurve;

        #endregion



        #region Public methods

        public void SetMaxParticlesPartEffect(float healthPart)
        {
            ParticleSystem.MainModule module = partsEffect.main;
            float partsToSpawn = hitCurve.Evaluate(healthPart);
            module.maxParticles = Mathf.RoundToInt(partsToSpawn);
        }

        #endregion
    }
}
