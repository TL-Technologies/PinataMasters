using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace Modules.Legacy.InternetReachability
{
    public class LLInternetReachabilityManager : SingletonMonoBehaviour<LLInternetReachabilityManager>
    {
        #region Fields

        public static event Action<NetworkReachability> OnInternetReachabilityChanged;

        [SerializeField] float secondsForCheckReachability = 3f;

        NetworkReachability internetReachability;
        float currentReachablilityCheckDuration = 0f;

        #endregion



        #region Properties

        NetworkReachability InternetReachability
        {
            set
            {
                if (internetReachability != value)
                {
                    internetReachability = value;

                    if (OnInternetReachabilityChanged != null)
                    {
                        OnInternetReachabilityChanged(internetReachability);
                    }
                }
            }
        }

        #endregion



        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            internetReachability = Application.internetReachability;

            if (OnInternetReachabilityChanged != null)
            {
                OnInternetReachabilityChanged(internetReachability);
            }
        }


        void Update()
        {
            currentReachablilityCheckDuration += Time.unscaledDeltaTime;

            if (currentReachablilityCheckDuration >= secondsForCheckReachability)
            {
                currentReachablilityCheckDuration = 0f;

                InternetReachability = Application.internetReachability;
            }
        }

        #endregion
    }
}
