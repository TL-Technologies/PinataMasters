using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private AudioClip audioClip = null;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlaySound);
        }

        #endregion



        #region Private methods

        private void PlaySound()
        {
            AudioManager.Instance.Play(audioClip, AudioType.Sound);
        }

        #endregion
    }
}
