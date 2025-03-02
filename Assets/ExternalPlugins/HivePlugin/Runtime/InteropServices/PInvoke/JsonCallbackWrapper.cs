using Newtonsoft.Json;
using System;


namespace Modules.Hive.InteropServices.PInvoke
{
    public sealed class JsonCallbackWrapper<T> : StringCallbackWrapperBase
    {
        private readonly Action<T> callback;


        public JsonCallbackWrapper(Action<T> callback, CallbackWrapperOptions options = CallbackWrapperOptions.Default) : base(options)
        {
            this.callback = callback;
        }


        protected override void OnInvoke(string arg1)
        {
            if (callback == null)
            {
                return;
            }

            T obj = JsonConvert.DeserializeObject<T>(arg1);
            callback(obj);
        }
    }
}
