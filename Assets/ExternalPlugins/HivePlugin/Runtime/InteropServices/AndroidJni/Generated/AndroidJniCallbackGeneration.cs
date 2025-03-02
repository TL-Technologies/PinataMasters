
using System;
using UnityEngine;


// AUTO-GENERATED FILE. DO NOT MODIFY.
namespace Modules.Hive.InteropServices.AndroidJni
{
    #region BooleanCallbackWrapper

    public abstract class BooleanCallbackWrapperBase : AndroidJavaProxy
    {
        public BooleanCallbackWrapperBase(string interfaceName) : base(interfaceName) { }


        /// <summary>
        /// This method reflects the same method in Java code and is intended to handle an event from there.
        /// </summary>
        protected abstract void onComplete(bool arg1);
    }


    public sealed class BooleanCallbackWrapper : BooleanCallbackWrapperBase
    {
        private readonly Action<bool> callback;


        public BooleanCallbackWrapper(Action<bool> callback) : base("org.hive.foundation.callbacks.UnityBooleanCallback")
        {
            this.callback = callback;
        }


        public BooleanCallbackWrapper(string interfaceName, Action<bool> callback) : base(interfaceName)
        {
            this.callback = callback;
        }


        /// <inheritdoc/>
        protected override void onComplete(bool arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion



    #region Int32CallbackWrapper

    public abstract class Int32CallbackWrapperBase : AndroidJavaProxy
    {
        public Int32CallbackWrapperBase(string interfaceName) : base(interfaceName) { }


        /// <summary>
        /// This method reflects the same method in Java code and is intended to handle an event from there.
        /// </summary>
        protected abstract void onComplete(int arg1);
    }


    public sealed class Int32CallbackWrapper : Int32CallbackWrapperBase
    {
        private readonly Action<int> callback;


        public Int32CallbackWrapper(Action<int> callback) : base("org.hive.foundation.callbacks.UnityInt32Callback")
        {
            this.callback = callback;
        }


        public Int32CallbackWrapper(string interfaceName, Action<int> callback) : base(interfaceName)
        {
            this.callback = callback;
        }


        /// <inheritdoc/>
        protected override void onComplete(int arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion



    #region StringCallbackWrapper

    public abstract class StringCallbackWrapperBase : AndroidJavaProxy
    {
        public StringCallbackWrapperBase(string interfaceName) : base(interfaceName) { }


        /// <summary>
        /// This method reflects the same method in Java code and is intended to handle an event from there.
        /// </summary>
        protected abstract void onComplete(string arg1);
    }


    public sealed class StringCallbackWrapper : StringCallbackWrapperBase
    {
        private readonly Action<string> callback;


        public StringCallbackWrapper(Action<string> callback) : base("org.hive.foundation.callbacks.UnityStringCallback")
        {
            this.callback = callback;
        }


        public StringCallbackWrapper(string interfaceName, Action<string> callback) : base(interfaceName)
        {
            this.callback = callback;
        }


        /// <inheritdoc/>
        protected override void onComplete(string arg1)
        {
            callback?.Invoke(arg1);
        }
    }

    #endregion



}
