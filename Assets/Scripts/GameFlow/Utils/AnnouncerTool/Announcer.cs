using DG.Tweening;
using Modules.General.Obsolete;
using System.Collections;
using UnityEngine;

namespace PinataMasters
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Announcer : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;


        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public void Init(Sprite sprite, Vector3 startPosition, Vector3 finishPosition, AnimationCurve moveCurve, AnimationCurve scaleCurve, AnimationCurve alphaCurve, float lifeTime)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
            transform.localScale = Vector3.one;

            if (moveCurve != null)
            {
                transform.position = startPosition;
                transform.DOMove(finishPosition, lifeTime).SetEase(moveCurve);
            }

            if (scaleCurve != null)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, lifeTime).SetEase(scaleCurve);
            }

            if (alphaCurve != null)
            {
                spriteRenderer.DOFade(0f, lifeTime).SetEase(alphaCurve);
            }

            StartCoroutine(Hide(lifeTime));
        }


        private IEnumerator Hide(float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);

            gameObject.ReturnToPool();
        }
    }
}
