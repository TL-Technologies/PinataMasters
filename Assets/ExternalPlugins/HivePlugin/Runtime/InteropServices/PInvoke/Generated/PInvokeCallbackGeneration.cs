

using System;
using System.Runtime.InteropServices;


// AUTO-GENERATED FILE. DO NOT MODIFY.
namespace Modules.Hive.InteropServices.PInvoke
{
    #region BooleanCallback

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct BooleanCallback
    {
        private readonly IntPtr ptr;
        private readonly Action<IntPtr, bool> callback;


        internal BooleanCallback(IntPtr ptr, Action<IntPtr, bool> callback)
        {
            this.ptr = ptr;
            this.callback = callback;
        }


        /// <summary>
        /// <para>
        /// Creates a native callback struct for a managed static method.
        /// </para>
        /// <para>
        /// KEEP IN MIND that only static methods can be passed to argument <paramref name="staticMethod"/>!
        /// </para>
        /// </summary>
        /// <param name="staticMethod">A delegate to a static method.</param>
        /// <returns>A native callback struct for a managed static method.</returns>
        public static BooleanCallback ForStaticMethod(Action<IntPtr, bool> staticMethod)
        {
            return new BooleanCallback(IntPtr.Zero, staticMethod);
        }
    }


    public abstract class BooleanCallbackWrapperBase : CallbackWrapper
    {
        protected abstract void OnInvoke(bool arg1);


        public BooleanCallbackWrapperBase(CallbackWrapperOptions options) : base(options) { }


        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, bool>))]
        private static void Invoke(IntPtr ptr, bool arg1)
        {
            var wrapper = FromIntPtr<BooleanCallbackWrapperBase>(ptr);
            wrapper?.OnInvoke(arg1);
        }


        public BooleanCallback GetCallback()
        {
            return new BooleanCallback(ToIntPtr(), Invoke);
        }


        public static implicit operator BooleanCallback(BooleanCallbackWrapperBase wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            return wrapper.GetCallback();
        }
    }


    public sealed class BooleanCallbackWrapper : BooleanCallbackWrapperBase
    {
        private readonly Action<bool> callback;


        public BooleanCallbackWrapper(
            Action<bool> callback, 
            CallbackWrapperOptions options = CallbackWrapperOptions.Default) : 
            base(options)
        {
            this.callback = callback;
        }


        protected override void OnInvoke(bool arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion


    #region Int32Callback

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Int32Callback
    {
        private readonly IntPtr ptr;
        private readonly Action<IntPtr, int> callback;


        internal Int32Callback(IntPtr ptr, Action<IntPtr, int> callback)
        {
            this.ptr = ptr;
            this.callback = callback;
        }


        /// <summary>
        /// <para>
        /// Creates a native callback struct for a managed static method.
        /// </para>
        /// <para>
        /// KEEP IN MIND that only static methods can be passed to argument <paramref name="staticMethod"/>!
        /// </para>
        /// </summary>
        /// <param name="staticMethod">A delegate to a static method.</param>
        /// <returns>A native callback struct for a managed static method.</returns>
        public static Int32Callback ForStaticMethod(Action<IntPtr, int> staticMethod)
        {
            return new Int32Callback(IntPtr.Zero, staticMethod);
        }
    }


    public abstract class Int32CallbackWrapperBase : CallbackWrapper
    {
        protected abstract void OnInvoke(int arg1);


        public Int32CallbackWrapperBase(CallbackWrapperOptions options) : base(options) { }


        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, int>))]
        private static void Invoke(IntPtr ptr, int arg1)
        {
            var wrapper = FromIntPtr<Int32CallbackWrapperBase>(ptr);
            wrapper?.OnInvoke(arg1);
        }


        public Int32Callback GetCallback()
        {
            return new Int32Callback(ToIntPtr(), Invoke);
        }


        public static implicit operator Int32Callback(Int32CallbackWrapperBase wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            return wrapper.GetCallback();
        }
    }


    public sealed class Int32CallbackWrapper : Int32CallbackWrapperBase
    {
        private readonly Action<int> callback;


        public Int32CallbackWrapper(
            Action<int> callback, 
            CallbackWrapperOptions options = CallbackWrapperOptions.Default) : 
            base(options)
        {
            this.callback = callback;
        }


        protected override void OnInvoke(int arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion


    #region StringCallback

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct StringCallback
    {
        private readonly IntPtr ptr;
        private readonly Action<IntPtr, string> callback;


        internal StringCallback(IntPtr ptr, Action<IntPtr, string> callback)
        {
            this.ptr = ptr;
            this.callback = callback;
        }


        /// <summary>
        /// <para>
        /// Creates a native callback struct for a managed static method.
        /// </para>
        /// <para>
        /// KEEP IN MIND that only static methods can be passed to argument <paramref name="staticMethod"/>!
        /// </para>
        /// </summary>
        /// <param name="staticMethod">A delegate to a static method.</param>
        /// <returns>A native callback struct for a managed static method.</returns>
        public static StringCallback ForStaticMethod(Action<IntPtr, string> staticMethod)
        {
            return new StringCallback(IntPtr.Zero, staticMethod);
        }
    }


    public abstract class StringCallbackWrapperBase : CallbackWrapper
    {
        protected abstract void OnInvoke(string arg1);


        public StringCallbackWrapperBase(CallbackWrapperOptions options) : base(options) { }


        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, string>))]
        private static void Invoke(IntPtr ptr, string arg1)
        {
            var wrapper = FromIntPtr<StringCallbackWrapperBase>(ptr);
            wrapper?.OnInvoke(arg1);
        }


        public StringCallback GetCallback()
        {
            return new StringCallback(ToIntPtr(), Invoke);
        }


        public static implicit operator StringCallback(StringCallbackWrapperBase wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            return wrapper.GetCallback();
        }
    }


    public sealed class StringCallbackWrapper : StringCallbackWrapperBase
    {
        private readonly Action<string> callback;


        public StringCallbackWrapper(
            Action<string> callback, 
            CallbackWrapperOptions options = CallbackWrapperOptions.Default) : 
            base(options)
        {
            this.callback = callback;
        }


        protected override void OnInvoke(string arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion


}
