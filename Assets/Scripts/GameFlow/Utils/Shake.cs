using System.Collections;
using UnityEngine;


namespace PinataMasters
{
    public class Shake
    {

        #region Variables

        private const float DOUBLE_PI = Mathf.PI * 2f;

        private readonly Coroutine coroutine;

        private Transform shakeObjectTransform;
        private Vector3 shakeObjectStartPosition;

        #endregion


        #region Public methods

        public static Shake Play(Transform transform, float duration, float magnitude, AnimationCurve curve)
        {
            return new Shake(transform, duration, magnitude, curve);
        }


        public void Stop()
        {
            coroutine.StopCoroutine();
            shakeObjectTransform.position = shakeObjectStartPosition;
        }

        #endregion


        #region Private methods

        private Shake(Transform transform, float duration, float magnitude, AnimationCurve curve)
        {
            coroutine = Sheduler.PlayCoroutine(Run(transform, duration, magnitude, curve));
        }


        private IEnumerator Run(Transform transform, float duration, float magnitude, AnimationCurve curve)
        {
            float time = 0f;

            shakeObjectTransform = transform;
            shakeObjectStartPosition = transform.localPosition;

            while (time < duration)
            {
                transform.localPosition = shakeObjectStartPosition + RandomUnitCircle() * magnitude * curve.Evaluate(time / duration);

                time += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = shakeObjectStartPosition;
        }


        private Vector3 RandomUnitCircle()
        {
            float angle = Random.Range(0f, DOUBLE_PI);
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            return new Vector3(x, y, 0f);
        }

        #endregion
    }
}
