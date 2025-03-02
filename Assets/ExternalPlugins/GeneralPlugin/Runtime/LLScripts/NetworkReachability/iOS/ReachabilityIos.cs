#if UNITY_IOS
using AOT;
using System;
using System.Runtime.InteropServices;


namespace Modules.Networking
{
    public class ReachabilityIos : Reachability
    {
        #region Fields

        [DllImport("__Internal")]
        static extern void LLNetworkReachabilityInitWithHostName(string hostName,
            bool reachableOnWwan, Action<int> networkReachabilityCallback);

        [DllImport("__Internal")]
        static extern void LLNetworkReachabilityForInternetConnection(bool reachableOnWwan, 
            Action<int> networkReachabilityCallback);

        [DllImport("__Internal")]
        static extern int LLNetworkReachabilityNetworkStatus();

        #endregion



        #region Class lifecycle

        public ReachabilityIos(Action<NetworkStatus> networkStatusSetter) : base(networkStatusSetter)
        {
            LLNetworkReachabilityForInternetConnection(true, NetworkReachabilityChanged);
        }

        #endregion



        #region Public methods

        public override NetworkStatus GetNetworkStatus()
        {
            return (NetworkStatus)LLNetworkReachabilityNetworkStatus();
        }

        #endregion



        #region Native callbacks

        [MonoPInvokeCallback(typeof(Action<int>))]
        static void NetworkReachabilityChanged(int networkStatus)
        {
            networkStatusSetter((NetworkStatus)networkStatus);
        }

        #endregion
    }
}
#endif