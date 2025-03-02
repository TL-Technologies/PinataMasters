using UnityEngine;


namespace PinataMasters
{
    public class AutoplayEffect : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private ParticleSystem effect = null;

        #endregion



        #region Unity lifecycle

        private void Start()
        {
            if (effect != null)
            {
                Instantiate(effect, transform).Play(true);
            }
        }

        #endregion
    }
}
