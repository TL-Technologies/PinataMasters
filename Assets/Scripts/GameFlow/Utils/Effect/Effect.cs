using Modules.General.Obsolete;
using UnityEngine;


namespace PinataMasters
{
    public abstract class Effect : MonoBehaviour
    {
        public abstract void Play(Vector3 position);

        protected void Stop()
        {
            gameObject.ReturnToPool();
        }
    }
}
