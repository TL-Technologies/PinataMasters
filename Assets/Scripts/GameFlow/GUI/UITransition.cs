using UnityEngine;
using Modules.Legacy.Tweens;
using System;


namespace PinataMasters
{
    public class UITransition : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private TweenScale tweenScale = null;
        [SerializeField]
        private TweenPosition tweenPosition = null;
        [SerializeField]
        private GameObject model = null;


        private Action decreaseCallback;
        private Action increaseCallback;

        private uint currentLevel;

        #endregion



        #region Properties

        public bool NeedShow
        {
            get
            {
                return currentLevel < Player.Level;
            }
        }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            currentLevel = Player.Level;
        }

        #endregion



        #region Public methods

        public void DecreaseScale(Action callbackOnDecrease, Action callbackOnIncrease)
        {
            decreaseCallback = callbackOnDecrease;
            increaseCallback = callbackOnIncrease;

            model.SetActive(true);

            tweenScale.AddOnFinishedDelegate(DecreaseScaleCallback);
            tweenScale.ResetTween();
            tweenScale.Play(true);

            tweenScale.ResetTween();
            tweenPosition.Play(true);

            currentLevel = Player.Level;
        }


        public void IncreaseScale()
        {
            tweenScale.RemoveOnFinishedDelegate(DecreaseScaleCallback);
            tweenScale.AddOnFinishedDelegate(FinishCallback);
            tweenScale.ResetTween();
            tweenScale.Play(false);

            tweenScale.ResetTween();
            tweenPosition.Play(false);
        }

        #endregion



        #region Private methods

        private void DecreaseScaleCallback(ITweener tw)
        {
            decreaseCallback();
            IncreaseScale();
        }


        private void FinishCallback(ITweener tw)
        {
            tweenScale.RemoveOnFinishedDelegate(FinishCallback);
            model.SetActive(false);

            increaseCallback();
        }

        #endregion
    }
}