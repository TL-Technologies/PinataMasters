using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Networking
{
    public class ReachabilityEditorHandler
    {
        #region Fields

        const float NetworkReachabilityCheckFrequency = 1f;

        NetworkReachability applicationNetworkReachability;
        CancellationTokenSource cancellationTokenSource = null;
        ReachabilityEditor reachability;

        #endregion



        #region Properties

        public NetworkReachability ApplicationNetworkReachability
        {
            get
            {
                return applicationNetworkReachability;
            }
            private set
            {
                if (applicationNetworkReachability != value)
                {
                    applicationNetworkReachability = value;

                    reachability.InternetReachabilityChanged(applicationNetworkReachability);
                }
            }
        }

        #endregion



        #region Public methods

        public void Initialize(ReachabilityEditor reachabilityEditor)
        {
            reachability = reachabilityEditor;
            applicationNetworkReachability = Application.internetReachability;

            cancellationTokenSource = new CancellationTokenSource();
            CheckNetworkReachability(cancellationTokenSource.Token, NetworkReachabilityCheckFrequency);
        }

        #endregion



        #region Private methods
        
        async void CheckNetworkReachability(CancellationToken token, float frequency)
        {
            while (!token.IsCancellationRequested)
            {
                ApplicationNetworkReachability = Application.internetReachability;
                await Task.Delay(TimeSpan.FromSeconds(frequency), token);
            }
        }
        
        
        void StopChecking()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        #endregion
    }
}
