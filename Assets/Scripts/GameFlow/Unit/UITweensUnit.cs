using System;
using UnityEngine;

namespace PinataMasters
{
    public class UITweensUnit<T> : UIUnit<T> where T : UnitResult
    {
        [Header("Animation")]
        [SerializeField]
        [Range(0f, float.MaxValue)]
        protected float durationShow = 0f;
        [SerializeField]
        [Range(0f, float.MaxValue)]
        protected float durationHide = 0f;
        [SerializeField]
        private ShowHideTweens[] tweens = null;

        protected override void Awake()
        {
            base.Awake();

            foreach (var tween in tweens)
            {
                tween.Show.Duration = durationShow;
                tween.Hide.Duration = durationHide;
            }
        }

        public override void Show(Action<T> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].Show.Play(i == 0 ? Showed : (TweenAnchorPosition.OnFinish)null);
            }
        }

        public override void Hide(T result = null)
        {
            base.Hide(result);

            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].Hide.Play(i == 0 ? () => Hided(result) : (TweenAnchorPosition.OnFinish)null);
            }
        }
    }
}
