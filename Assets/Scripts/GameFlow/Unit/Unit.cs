using System;
using UnityEngine;

namespace PinataMasters
{
    public class UnitResult {}

    public class Unit<T> : MonoBehaviour where T : UnitResult
    {
        private Action<T> onHided = delegate { };
        private Action onShowed = delegate { };

        public virtual void Show(Action<T> onHided = null, Action onShowed = null)
        {
            this.onHided = onHided ?? delegate { };
            this.onShowed = onShowed ?? delegate { };
        }

        protected virtual void Hided(T result = null)
        {
            onHided(result);
        }


        protected virtual void Showed()
        {
            onShowed();
        }
    }
}
