using Modules.General.Obsolete;
using System.Collections;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailEffect : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private TrailRenderer trail = null;

        [SerializeField]
        private float delayAfterShellDestroy = 0f;

        #endregion



        #region Public methods

        public void Init(Gradient gradient)
        {
            trail.Clear();
            trail.colorGradient = gradient;
        }


        public void DisableAfterDelay()
        {
            StartCoroutine(ReturnToPool());
        }

        #endregion



        #region Private methods

        private IEnumerator ReturnToPool()
        {
            yield return new WaitForSeconds(delayAfterShellDestroy);
            gameObject.ReturnToPool();
        }

        #endregion
    }
}
