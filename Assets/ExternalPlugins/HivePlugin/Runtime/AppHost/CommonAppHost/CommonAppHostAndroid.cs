#if UNITY_ANDROID
using System.Threading.Tasks;
using Modules.Hive.Ioc;
using UnityEngine;

namespace Modules.Hive
{
    internal class CommonAppHostAndroid : CommonAppHost
    {
        private AndroidJavaClass appHostClass = null;
        

        public CommonAppHostAndroid(
            IServiceContainer serviceContainer, 
            IEventAggregator eventAggregator, 
            AppLifecycleDispatcher appLifecycleDispatcher, 
            AppHostPluginsHub plugins) : 
            base(serviceContainer, eventAggregator, appLifecycleDispatcher, plugins) { }
        

        protected override Task RunAsyncImpl()
        {
            appHostClass = new AndroidJavaClass("org.hive.foundation.apphost.AppHost");

            // GooglePlayAppProfile profile = AppProfile.GooglePlay;

            // // Set Google Play Application Public Key
            // string s = profile.EncodedApplicationPublicKey;
            // if (!string.IsNullOrEmpty(s))
            // {
            //     appHostClass.CallStatic("setEncodedApplicationPublicKey", s);

            //     profile.ApplicationPublicKey = appHostClass.CallStatic<string>("getApplicationPublicKey");
            // }

            // Run native plugin
            appHostClass.CallStatic("run");

            return Task.CompletedTask;
        }
    }
}
#endif
