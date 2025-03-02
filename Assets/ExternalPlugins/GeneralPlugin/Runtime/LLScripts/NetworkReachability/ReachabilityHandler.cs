using System;


namespace Modules.Networking
{
    public class ReachabilityHandler
    {
        #region Fields

        static ReachabilityHandler instance;

        public event Action<NetworkStatus> NetworkReachabilityStatusChanged;
    
        NetworkStatus networkStatus;

        #endregion



        #region Properties

        public static ReachabilityHandler Instance => instance ?? (instance = new ReachabilityHandler());

        
        public NetworkStatus NetworkStatus
        {
            get
            {
                return networkStatus;
            }
            private set
            {
                if (networkStatus != value)
                {
                    networkStatus = value;

                    NetworkReachabilityStatusChanged?.Invoke(networkStatus);
                }
            }
        }

        #endregion



        #region Class lifecycle

        public ReachabilityHandler()
        {
            Reachability reachability = null;
            #if UNITY_EDITOR || UNITY_STANDALONE
                reachability = new ReachabilityEditor(SetNetworkStatus);
            #elif UNITY_IOS
                reachability = new ReachabilityIos(SetNetworkStatus);
            #elif UNITY_ANDROID
                reachability = new ReachabilityAndroid(SetNetworkStatus);
            #endif
            
            networkStatus = reachability.GetNetworkStatus();
        }

        #endregion



        #region Private Methods

        void SetNetworkStatus(NetworkStatus status)
        {
            NetworkStatus = status;
        }
        
        #endregion
    }
}
