using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    [RequireComponent(typeof(Button))]
    public class ButtonVibration : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private HapticTypes onClickVibration = HapticTypes.None;

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
            VibrationManager.Instance.PlayVibration(onClickVibration);
        }

        #endregion

    }
}
