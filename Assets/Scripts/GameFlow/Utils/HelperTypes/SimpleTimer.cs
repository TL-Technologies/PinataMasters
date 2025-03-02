using UnityEngine;
using System;


namespace PinataMasters.HelperTypes
{
    public class SimpleTimer
    {
        #region Fields

        Action callback;

        #endregion



        #region Properties

        public float RemainingTime { get; private set; }

        #endregion



        #region Class lifecycle

        public SimpleTimer(float _duration, Action _callback)
        {
            ResetTimer(_duration, _callback);
        }

        #endregion



        #region Public methods

        public void CustomUpdate(float _delay)
        {
            if (RemainingTime > 0f)
            {
                RemainingTime -= _delay;

                if (RemainingTime < 0f)
                {
                    RemainingTime = 0f;
                    callback?.Invoke();
                }
            }
        }


        public void ResetTimer(float _duration, Action _callback)
        {
            RemainingTime = _duration;
            callback = _callback;

            if (Mathf.Approximately(_duration, 0f))
            {
                callback?.Invoke();
            }
        }

        #endregion
    }
}
