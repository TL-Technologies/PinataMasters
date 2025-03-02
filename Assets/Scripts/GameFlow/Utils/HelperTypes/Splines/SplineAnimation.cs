using UnityEngine;
using System;


namespace PinataMasters
{
    public class SplineAnimation
    {
        #region Fields

        Spline spline;
        Transform transform;
        float duration;
        Action callback;

        float time;

        #endregion



        #region Properties

        public float RelativeTime => Mathf.Clamp01(time / duration);

        #endregion



        #region Class lifecycle

        public SplineAnimation(Spline _spline, Transform _transform, float _duration, Action _callback = null)
        {
            spline = _spline;
            transform = _transform;
            duration = _duration;
            callback = _callback;

            time = 0f;
        }

        #endregion


        #region Public methods

        public void CustomUpdate(float _deltaTime)
        {
            if (time < duration)
            {
                time += _deltaTime;

                transform.localPosition = spline.GetSplinePoint(RelativeTime);

                if (time > duration)
                {
                    callback?.Invoke();
                }
            }
        }

        #endregion
    }
}
