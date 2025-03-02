using System;
using UnityEngine;


namespace PinataMasters
{
    public class MileyEmotions : PinataEmotions
    {
        #region Variables

        [SerializeField]
        private GameObject ball = null;

        #endregion


        #region Private methods

        public override void PlayFailAnimation(Action OnFinish)
        {
            base.PlayFailAnimation(OnFinish);

            ball.SetActive(false);
        }

        #endregion
    }
}
