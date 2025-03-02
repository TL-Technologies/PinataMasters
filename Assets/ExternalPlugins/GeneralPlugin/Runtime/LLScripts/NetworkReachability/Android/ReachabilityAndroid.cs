#if UNITY_ANDROID
using System;


namespace Modules.Networking
{
    public class ReachabilityAndroid : Reachability
    {
        #region Fields

        const string ClassName = "LLNetworkReachability";
        const string MethodInit = "LLNetworkReachabilityInit";
        const string MethodGetNetworkStatus = "LLNetworkReachabilityNetworkStatus";

        #endregion



        #region Class lifecycle

        public ReachabilityAndroid(Action<NetworkStatus> networkStatusSetter) : base(networkStatusSetter)
        {
            LLAndroidJavaSingletone<ReachabilityAndroid>.RegisterClassName(ClassName);
            LLAndroidJavaSingletone<ReachabilityAndroid>.CallStatic(MethodInit, 
                LLAndroidJavaCallback.ProxyCallback(NetworkReachabilityChanged));
        }

        #endregion



        #region Public methods

        public override NetworkStatus GetNetworkStatus()
        {
            return (NetworkStatus)LLAndroidJavaSingletone<ReachabilityAndroid>.CallStatic<int>(MethodGetNetworkStatus);
        }

        #endregion



        #region Native callbacks

        static void NetworkReachabilityChanged(int networkStatus)
        {
            networkStatusSetter((NetworkStatus)networkStatus);
        }

        #endregion
    }
}
#endif