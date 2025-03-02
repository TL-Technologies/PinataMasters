using System;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(Canvas))]
    public class UIUnit<T> : Unit<T> where T : UnitResult
    {
        protected virtual void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }


        public override void Show(Action<T> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);
            gameObject.SetActive(true);
            EventSystemController.DisableEventSystem();
        }


        public virtual void Hide(T result = null)
        {
            EventSystemController.DisableEventSystem();
        }
        
        protected override void Showed()
        {
            EventSystemController.EnableEventSystem();
            base.Showed();
        }


        protected override void Hided(T result = null)
        {
            EventSystemController.EnableEventSystem();
            gameObject.SetActive(false);
            base.Hided(result);
        }
    }
}
