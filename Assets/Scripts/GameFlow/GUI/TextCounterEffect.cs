using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    [RequireComponent(typeof(Text))]
    public class TextCounterEffect : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private Text text = null;
        [SerializeField]
        private float duration = 1f;

        private float currentGems;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            currentGems = Player.Gems;
        }

        #endregion



        #region Public methods

        public void Refresh(float newValue)
        {
            if (Mathf.Approximately(currentGems, newValue) || newValue < currentGems || !gameObject.activeInHierarchy)
            {
                text.text = newValue.ToShortFormat();
                return;
            }

            StartCoroutine(Counter(newValue));
        }

        #endregion



        #region Private methods

        private IEnumerator Counter(float newGemsValue)
        {
            float amountPerSecond = (newGemsValue - currentGems) / duration;

            while (currentGems < newGemsValue)
            {
                currentGems += amountPerSecond * Time.deltaTime;
                text.text = currentGems.ToShortFormat();

                yield return null;
            }

            currentGems = newGemsValue;
            text.text = currentGems.ToShortFormat();
        }

        #endregion
    }
}
