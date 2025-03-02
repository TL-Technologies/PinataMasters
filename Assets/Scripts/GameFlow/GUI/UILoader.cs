using System;
using DG.Tweening;
using UnityEngine;


namespace PinataMasters
{
    public class UILoader : UIUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UILoader> Prefab = new ResourceGameObject<UILoader>("Game/GUI/DialogLoader");

        [SerializeField]
        private TweenImageColor tweenColor = null;
        [SerializeField]
        private float loaderDuration = 0f;
        [SerializeField]
        private RectTransform loader = null;

        #endregion



        #region Public methods

        protected override void Awake()
        {
            base.Awake();
            loader.DORotate(new Vector3(0f, 0f, 360f), loaderDuration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }


        public void Show()
        {
            base.Show();
            tweenColor.Play(() => Showed());
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);
            tweenColor.Play(() => Hided(), false);
        }

        #endregion
    }
}
