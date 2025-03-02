using  System;


namespace Modules.Networking
{
    public abstract class Reachability
    {
        #region Fields

        protected static Action<NetworkStatus> networkStatusSetter;

        #endregion



        #region Class lifecycle

        public Reachability(Action<NetworkStatus> networkStatusSetter)
        {
            Reachability.networkStatusSetter = networkStatusSetter;
        }

        #endregion



        #region Public methods

        public abstract NetworkStatus GetNetworkStatus();

        #endregion
    }
}
