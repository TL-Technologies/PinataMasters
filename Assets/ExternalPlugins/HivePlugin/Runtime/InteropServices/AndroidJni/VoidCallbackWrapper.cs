using System;
using UnityEngine;


namespace Modules.Hive.InteropServices.AndroidJni
{
    public abstract class VoidCallbackWrapperBase : AndroidJavaProxy
    {
        public VoidCallbackWrapperBase() : base("org.hive.foundation.callbacks.UnityVoidCallback") { }


        public VoidCallbackWrapperBase(string interfaceName) : base(interfaceName) { }


        /// <summary>
        /// This method reflects the same method in Java code and is intended to handle an event from there.
        /// </summary>
        protected abstract void onComplete();
    }


    public class VoidCallbackWrapper : VoidCallbackWrapperBase
    {
        private readonly Action callback;


        public VoidCallbackWrapper(Action callback)
        {
            this.callback = callback;
        }


        public VoidCallbackWrapper(string interfaceName, Action callback) : base(interfaceName)
        {
            this.callback = callback;
        }


        /// <inheritdoc/>
        protected override void onComplete()
        {
            callback?.Invoke();
        }
    }
}
