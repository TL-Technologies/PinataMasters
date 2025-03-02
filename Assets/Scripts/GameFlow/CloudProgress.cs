using MiniJSON;
using Modules.General;
using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters 
{
    public static class CloudProgress
    {
        [Serializable]
        public class Data
        {
            public Player.Data Player;
            public TutorialManager.Data Tutorial;
            public List<string> IDFA = new List<string>();
        }

        #region Variables

        private const string IS_SYNCHRONIZE_ENABLED = "is_synchronize_enabled";

        #endregion



        #region Properties

        public static bool IsSynchronizeEnabled
        {
            get
            {
                return CustomPlayerPrefs.GetBool(IS_SYNCHRONIZE_ENABLED, true);
            }
            set
            {
                CustomPlayerPrefs.SetBool(IS_SYNCHRONIZE_ENABLED, value);
            }
        }


        public static bool IsUserLoggedIn
        {
            get
            {
                return false;//FirebaseAnalyticsServiceImplementor.IsLoggedIn && LLFacebookManager.IsLogin();
            }
        }


        public static bool IsBlockedByInterstitial
        {
            get;
            set;
        }


        public static bool IsBlockedBySubscription
        {
            get;
            set;
        }


        public static bool IsLoginInProcess
        {
            get;
            set;
        }

        #endregion



        #region Public methods

        public static void Initialize()
        {
            LLApplicationStateRegister.OnApplicationEnteredBackground += (backgroud) =>
            {
                if (backgroud)
                {
                    Save();
                }
            };

            // if (LLFacebookManager.IsLogin())
            // {
            //     FirebaseAnalyticsServiceImplementor.SignIn(LLFacebookManager.GetCurrentUserToken(), null);
            // }
        }


        public static void TryShowRestoreProgress()
        {
            if (IsSynchronizeEnabled && IsUserLoggedIn && !IsBlockedByInterstitial && !IsBlockedBySubscription && !IsLoginInProcess)
            {
//                FirebaseAnalyticsServiceImplementor.GetUserData(StartWork);
            }
        }


        public static void LoginFacebook(Action callback)
        {
            IsLoginInProcess = true;

            UILoader.Prefab.Instance.Show();

            // LLFacebookManager.LoginWithGetData((_) =>
            // {
            //     UILoader.Prefab.Instance.Hide();
            //
            //     if (LLFacebookManager.IsLogin())
            //     {
            //         FirebaseAnalyticsServiceImplementor.SignIn(LLFacebookManager.GetCurrentUserToken(), (signInCallback) =>
            //         {
            //             if (Application.internetReachability != NetworkReachability.NotReachable && FirebaseAnalyticsServiceImplementor.IsLoggedIn)
            //             {
            //                 FirebaseAnalyticsServiceImplementor.GetUserData(StartWork);
            //             }
            //             callback();
            //         });
            //     }
            //     else
            //     {
                    callback();
            //     }
            // });
        }


        public static void Save()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || !IsUserLoggedIn)
            {
                return;
            }

//            FirebaseAnalyticsServiceImplementor.GetUserData((currentData) =>
//            {
//                Data newData = new Data
//                {
//                    Player = Player.GetPrefs(),
//                    Tutorial = TutorialManager.Instance.GetPrefs(),
//                };
//
//                if (!string.IsNullOrEmpty(currentData))
//                {
//                    Data remoteData = JsonConvert.DeserializeObject<Data>(currentData);
//
//                    #if UNITY_IOS
//                    string IDFA = UnityEngine.iOS.Device.advertisingIdentifier;
//
//                    if (!remoteData.IDFA.Exists(i => i == IDFA))
//                    {
//                        remoteData.IDFA.Add(IDFA);
//                    }
//                    #endif
//                    newData.IDFA = remoteData.IDFA;
//                }
//
//                string dataToSave = JsonConvert.SerializeObject(newData);
//                FirebaseAnalyticsServiceImplementor.SetUserData(dataToSave, null);
//            });
        }

        #endregion



        #region Private methods

        private static void StartWork(string remoteData)
        {
            if (string.IsNullOrEmpty(remoteData))
            {
                Save();
                return;
            }

            Data data = JsonConvert.DeserializeObject<Data>(remoteData);
            if (Player.IsBetterCurrent(data.Player))
            {
                if (IsSynchronizeEnabled)
                {
                    UILoadProgress.Prefab.Instance.Show(data);
                }
                return;
            }

            Save();
        }

        #endregion
    }
}
