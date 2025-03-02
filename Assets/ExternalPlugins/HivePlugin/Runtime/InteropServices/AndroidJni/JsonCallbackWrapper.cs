using Newtonsoft.Json;
using System;


namespace Modules.Hive.InteropServices.AndroidJni
{
    public class JsonCallbackWrapper<T> : StringCallbackWrapperBase
    {
        private readonly Action<T> callback;


        public JsonCallbackWrapper(Action<T> callback) : base("org.hive.foundation.callbacks.UnityJsonCallback")
        {
            this.callback = callback;
        }


        protected override void onComplete(string arg1)
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
