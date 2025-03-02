using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    [RequireComponent(typeof(Button))]
    public class ButtonEffect : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private ParticleSystem onClickEffect = null;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlayEffect);
        }

        #endregion



        #region Private methods

        private void PlayEffect()
        {
            if (onClickEffect != null)
            {
                onClickEffect.Play(true);
            }
        }

        #endregion

    }
}
