using DG.Tweening;
using Modules.General.Obsolete;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PinataMasters
{

    [RequireComponent(typeof(TextMeshPro))]
    public class IngameCurrencyCollectText : MonoBehaviour
    {
        #region Variables

        const string format = "+{0}";

        [SerializeField]
        private float lifeTime = 0f;
        [SerializeField]
        private float finishPositionY = -2f;
        [SerializeField]
        private Color startColor = Color.white;
        [SerializeField]
        private Color finishColor = Color.white;

        private TextMeshPro text;
        private WaitForSeconds wait;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            text = GetComponent<TextMeshPro>();
            wait = new WaitForSeconds(lifeTime);
        }

        #endregion



        #region Public methods

        public void Init(Vector3 position, float price)
        {
            transform.position = position;
            transform.DOMoveY(finishPositionY, lifeTime);

            text.color = startColor;
            text.text = string.Format(format, price.ToShortFormat());
            text.DOColor(finishColor, lifeTime);

            StartCoroutine(Release());
        }

        #endregion



        #region Private methods

        private IEnumerator Release()
        {
            yield return wait;

            gameObject.ReturnToPool();
        }

        #endregion
    }
}
