using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Networking
{
    public class ReachabilityEditor : Reachability
    {
        #region Fields

        readonly Dictionary<NetworkReachability, NetworkStatus> NetworkStatusByType = 
            new Dictionary<NetworkReachability, NetworkStatus>
        {
            { NetworkReachability.NotReachable, NetworkStatus.NotReachable },
            { NetworkReachability.ReachableViaCarrierDataNetwork, NetworkStatus.ReachableViaWwan },
            { NetworkReachability.ReachableViaLocalAreaNetwork, NetworkStatus.ReachableViaWiFi }
        };

        #endregion



        #region Class lifecycle

        public ReachabilityEditor(Action<NetworkStatus> networkStatusSetter) : base(networkStatusSetter)
        {
            ReachabilityEditorHandler reachabilityEditorHandler = new ReachabilityEditorHandler();
            reachabilityEditorHandler.Initialize(this);
        }

        #endregion



        #region Public methods

        public override NetworkStatus GetNetworkStatus()
        {
            return NetworkStatusByType[Application.internetReachability];
        }                   
      

        public void InternetReachabilityChanged(NetworkReachability internetReachability)
        {
            networkStatusSetter(NetworkStatusByType[internetReachability]);
        }

        #endregion
    }
}
