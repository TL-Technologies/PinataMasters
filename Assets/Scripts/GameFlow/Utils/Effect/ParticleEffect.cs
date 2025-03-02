using System.Collections;
using UnityEngine;


namespace PinataMasters
{

    public class ParticleEffect : Effect
    {
        #region Variables

        [SerializeField]
        private ParticleSystem effect;

        #endregion



        #region Public methods

        public override void Play(Vector3 position)
        {
            transform.position = position;
            effect.Play(true);

            StartCoroutine(StopParticleEffect());
        }


        public void StopEffect()
        {
            Stop();
        }

        #endregion



        #region Private methods

        private IEnumerator StopParticleEffect()
        {
            yield return new WaitForSeconds(effect.main.duration);
            Stop();
        }

        #endregion
    }
}
