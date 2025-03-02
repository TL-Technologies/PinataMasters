using System;


namespace Modules.Hive.InteropServices.PInvoke
{
    public abstract class VoidCallbackWrapperBase : CallbackWrapper
    {
        protected abstract void OnInvoke();


        public VoidCallbackWrapperBase(CallbackWrapperOptions options) : base(options) { }


        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr>))]
        private static void Invoke(IntPtr ptr)
        {
            var wrapper = FromIntPtr<VoidCallbackWrapperBase>(ptr);
            wrapper?.OnInvoke();
        }


        public VoidCallback GetCallback()
        {
            return new VoidCallback(ToIntPtr(), Invoke);
        }


        public static implicit operator VoidCallback(VoidCallbackWrapperBase wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            return wrapper.GetCallback();
        }
    }


    public sealed class VoidCallbackWrapper : VoidCallbackWrapperBase
    {
        private readonly Action callback;


        public VoidCallbackWrapper(Action callback, CallbackWrapperOptions options = CallbackWrapperOptions.Default) : base(options)
        {
            this.callback = callback;
        }
        

        protected override void OnInvoke()
        {
            callback?.Invoke();
        }
    }
}
