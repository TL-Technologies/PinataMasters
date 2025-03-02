using System;
using UnityEngine;


namespace Modules.Advertising
{
    public class SimpleTimer
    {
        #region Fields

        private Action completeCallback;

        #endregion



        #region Properties

        public float RemainingTime { get; private set; }

        #endregion



        #region Class lifecycle

        public SimpleTimer(float duration, Action callback)
        {
            ResetTimer(duration, callback);
        }

        #endregion



        #region Methods

        public void CustomUpdate(float delay)
        {
            if (RemainingTime > 0.0f)
            {
                RemainingTime -= delay;

                if (RemainingTime < 0.0f)
                {
                    RemainingTime = 0.0f;
                    completeCallback?.Invoke();
                }
            }
        }


        public void ResetTimer(float duration, Action callback)
        {
            RemainingTime = duration;
            completeCallback = callback;

            if (Mathf.Approximately(duration, 0.0f))
            {
                completeCallback?.Invoke();
            }
        }

        #endregion
    }
}
