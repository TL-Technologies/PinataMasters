using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.Hive.Ioc;
using Modules.InAppPurchase;
using Modules.Max;
using System;
using System.Collections;
using UnityEngine;


namespace PinataMasters
{
    public class Game : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private SpriteRenderer solidBack = null;
        [SerializeField]
        private SpriteRenderer solidFrontBack = null;

        private const string LAST_BACKGROUND_TIME = "last_background_time";
        private const float BACK_PROPOSAL_DELAY = 3f;

        private SceneArena sceneArena;
        private ScenePlayer scenePlayer;
        
        private bool isPersonalDataDeleted = false;

        private LoaderScreen loaderScreen = null;
        
        private ServiceInitialization serviceInitialization = new ServiceInitialization();

        #endregion



        #region Properties

        public bool IsPersonalDataDeleted
        {
            get
            {
                return isPersonalDataDeleted;
            }
            private set
            {
                if (isPersonalDataDeleted != value)
                {
                    isPersonalDataDeleted = value;
                }
            }
        }
        

        private DateTime LastBackgroundTime
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(LAST_BACKGROUND_TIME, DateTime.Now);
            }
            set
            {
                CustomPlayerPrefs.SetDateTime(LAST_BACKGROUND_TIME, value);
            }
        }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            loaderScreen = LoaderScreen.Prefab.Instance;
            loaderScreen.Show();
            loaderScreen.OnLoaderHide += LoaderScreen_OnLoaderHide;
            
            Application.targetFrameRate = 60;
            
            #if UNITY_ANDROID
                LLActivity.SetDebugEnable(CustomDebug.Enable);
            #endif

            #if !DEBUG_TARGET
                Input.multiTouchEnabled = false;
            #endif

            EventDispatcher.Subscribe<PrivacyPersonalDataDeletingDetected>(d => OnPersonalDataDeletingDetect());
        }


        private void Start()
        {
            serviceInitialization.OnServicesInitialized += ServiceInitialization_OnServicesInitialized;
            serviceInitialization.Initialize();
        }


        private void OnDestroy()
        {
            CustomAdvertisingManager.OnFullScreenAdStarted -= CustomAdvertisingManagerOnFullScreenAdStarted;
            CustomAdvertisingManager.OnFullScreenAdFinished -= CustomAdvertisingManagerOnFullScreenAdFinished;
            LLApplicationStateRegister.OnApplicationEnteredBackground -= TryProposeContent;
           
            loaderScreen.OnLoaderHide -= LoaderScreen_OnLoaderHide;
            
            serviceInitialization.OnServicesInitialized -= ServiceInitialization_OnServicesInitialized;
        }

        #endregion



        #region Private methods

        private void StartApplication()
        {
            ABTest.InGameAbTestData = new InGameAbTestData();
            ABTest.IsAbTestsInitialized = true;
            
            IAPs.Initialize();
            
            #if UNITY_IOS
                bool isSubscriptionActive = IAPs.IsSubscriptionActive; // this fix auto restore on iOS

                NotificationManager.Instance.Initialize();
            #endif
            

            CustomAdvertisingManager.OnFullScreenAdStarted += CustomAdvertisingManagerOnFullScreenAdStarted;
            CustomAdvertisingManager.OnFullScreenAdFinished += CustomAdvertisingManagerOnFullScreenAdFinished;
            
            // LLFacebookManager.Initialize();
            
            LLApplicationStateRegister.OnApplicationEnteredBackground += TryProposeContent;

            GameAnalytics.Initialize();
            Sessions.IncrementSession();

            SocialController.Initialize();

            AudioManager.Instance.Initialize();

            CloudProgress.Initialize();
           
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                Firebase.TrySendAppsflyerIDEvent();
            }, 3.0f);
            
            StartCoroutine(SilentLoadResources());

            if (!UISubscriptionStart.WasShowed)
            {
                if (IAPs.IsTrialSubscriptionAvailable && !IAPs.IsNoSubscriptionActive)
                {
                    UISubscriptionStart.Prefab.Instance.Show((_) => StartGame());
                }
                else
                {
                    UISubscriptionStart.WasShowed = true;
                    StartGame();
                }
            }
            else
            {
                StartGame();
            }
                            
            loaderScreen.Hide();
        }


        IEnumerator SilentLoadResources()
        {
            ArenaResource.Prefab.LoadValueAsync();
            UIPlayerMenu.Prefab.LoadValueAsync();
            IngameOffersController.Prefab.LoadValueAsync();
            UILevel.Prefab.LoadValueAsync();

            for (int i = 0; i < Arsenal.Count; i++)
            {
                yield return new WaitForSeconds(0.5f);
                Arsenal.GetWeaponConfig(i);
            }
        }
        

        void StartGame()
        {
            if (!Story.WasStoryShowed)
            {
                Story.Prefab.Instance.Show((_) => PlayArena());
                AudioManager.Instance.NormalizeAudio();
            }
            else if (!TutorialManager.Instance.IsShootTutorialPassed)
            {
                PlayArena();
            }
            else
            {
                ShowFoolishPopUp(SubscriptionPurchasePlacement.ApplicationStart, () => ShowDailyGift(() =>
                {
                    PlayArena();
                    ShowOfflineRewardPopUp(() => CloudProgress.TryShowRestoreProgress());
                }));
            }

            #if DEBUG_TARGET
                DevMenu.DevMenuManager.Initialize(new PinataDevContent());
            #endif
        }


        private void ShowFoolishPopUp(string placement, Action onHided = null)
        {
            onHided = onHided ?? delegate { };

            if (UISubscriptionSmart.IsFoolishPopUpAvailabe && !UISubscriptionStart.IsShowing && Application.internetReachability != NetworkReachability.NotReachable)
            {
                UISubscriptionSmart.Prefab.Instance.Show(placement, (_) => onHided?.Invoke());
            }
            else
            {
                onHided();
            }
        }


        private void OnPersonalDataDeletingDetect()
        {
            AdvertisingHelper.HideBanner();
            UIGDPRBack.Prefab.Instance.Show();
            AudioManager.Instance.Mute(true);

            IsPersonalDataDeleted = true;
        }


        private void TryProposeContent(bool isEnteredBackground)
        {
            TimeSpan intervalToCheck = DateTime.Now.Subtract(LastBackgroundTime);

            if (!isEnteredBackground && !IsPersonalDataDeleted && intervalToCheck.TotalSeconds > BACK_PROPOSAL_DELAY && ABTest.InGameAbTestData.isSubscriptionShowEnterBackground)
            {
                CloudProgress.IsBlockedBySubscription = true;
                ShowFoolishPopUp(SubscriptionPurchasePlacement.ApplicationEnteredBackground, () => CloudProgress.TryShowRestoreProgress());  
            }

            LastBackgroundTime = DateTime.Now;
        }


        private void ShowDailyGift(Action onHided)
        {
            if (DailyGifts.IsGiftAvailable)
            {
                UIDailyGift.Prefab.Instance.Show((_) => onHided?.Invoke());
            }
            else
            {
                onHided?.Invoke();
            }
        }


        private void ShowOfflineRewardPopUp(Action onHided)
        {
            AdvertisingHelper.ShowBanner();

            if (OfflineReward.OfflineCoinsBank > 1f)
            {
                TapZone.Lock(true);
                UIOfflineReward.Prefab.Instance.Show((_) =>
                {
                    TapZone.Lock(false);
                    onHided?.Invoke();
                });
            }
            else
            {
                onHided?.Invoke();
            }
        }


        private void PlayArena()
        {
            UIPlayerMenu.Prefab.Instantiate();
            SelectorLevels.GetLevels.TryLoadOutOfRangeLevel();
            sceneArena = ArenaResource.Prefab.Instance.SceneArena;
            scenePlayer = ArenaResource.Prefab.Instance.ScenePlayer;
            
            AdvertisingHelper.ShowBanner();
            sceneArena.Play(ShowScenePlayer);
        }


        private void ShowScenePlayer()
        {
            ABTest.SendTestRemote();
            scenePlayer.Show(PlayArena);
        }


        private void DisableOrEnableBack(bool isColored, Color colorToSet)
        {
            solidBack.gameObject.SetActive(isColored);
            solidBack.color = colorToSet;
        }


        private void DisableOrEnableFrontback(Color colorToSet)
        {
            solidFrontBack.color = colorToSet;
            solidFrontBack.gameObject.SetActive(!solidFrontBack.gameObject.activeSelf);
        }

        #endregion



        #region Events handler

        private void CustomAdvertisingManagerOnFullScreenAdFinished(AdModule adModule)
        {
            AudioManager.Instance.Mute(false);

            if (adModule == AdModule.Interstitial)
            {
                CloudProgress.IsBlockedByInterstitial = false;
                CloudProgress.TryShowRestoreProgress();
            }
        }

        
        private void CustomAdvertisingManagerOnFullScreenAdStarted(AdModule adModule)
        {
            AudioManager.Instance.Mute(true);

            if (adModule == AdModule.Interstitial)
            {
                CloudProgress.IsBlockedByInterstitial = true;
            }
        }
        
        
        private void LoaderScreen_OnLoaderHide()
        {
            Destroy(loaderScreen.gameObject);
        }

        
        private void ServiceInitialization_OnServicesInitialized()
        {
            serviceInitialization.OnServicesInitialized -= ServiceInitialization_OnServicesInitialized;

            StartApplication();
            
            Services.AdvertisingManager.CreateInactivityTimer(() =>
            {
                return ABTest.IsAbTestsInitialized && !Services.AdvertisingManager.IsFullScreenAdShowing;
            });
        }
        
        #endregion



        #region DebugGUI 

        private string level = "1";

        private void DebugGUI()
        {
            if (GUI.Button(new Rect(100f, 500f, 200f, 200f), "Reset Prefs"))
            {
                CustomPlayerPrefs.DeleteAll();
                Application.Quit();
            }

            if (GUI.Button(new Rect(500f, 500f, 200f, 200f), "Add Gold"))
            {
                Player.AddCoins(float.MaxValue / 100f);
            }

            if (GUI.Button(new Rect(700f, 100f, 200f, 200f), "Add Gems"))
            {
                Player.AddGems(float.MaxValue / 100f);
            }

            if (GUI.Button(new Rect(700f, 300f, 200f, 200f), "Subscription ON"))
            {
                IAPs.IsFakeSubscriptionActive = true;
            }

            if (GUI.Button(new Rect(700f, 500f, 200f, 200f), "Subscription OFF"))
            {
                IAPs.IsFakeSubscriptionActive = false;
            }

            if (GUI.Button(new Rect(100f, 700f, 200f, 200f), "Kill pinata"))
            {
                Pinata pinata = FindObjectOfType<Pinata>();
                if (pinata != null)
                {
                    pinata.KillPinata();
                }
            }

            if (GUI.Button(new Rect(500f, 300f, 200f, 200f), "UnlockWeapon"))
            {
                for (int i = 0; i < Arsenal.Count; i++)
                {
                    Player.BuyWeapon(i);
                }
                Player.AddCoins(1f);
            }

            if (GUI.Button(new Rect(500f, 100f, 200f, 200f), "UnlockSkin"))
            {
                for (int i = 0; i < Skins.Count; i++)
                {
                    Player.BuySkin(i);
                }
                Player.AddGems(1f);
            }

            if (GUI.Button(new Rect(100f, 300, 200f, 200f), "Announcers: " + UIAnnouncers.ShouldShow))
            {
                UIAnnouncers.ShouldShow = !UIAnnouncers.ShouldShow;
            }

            level = GUI.TextArea(new Rect(300f, 900f, 200f, 200f), level, 100);

            if (GUI.Button(new Rect(100f, 900f, 200f, 200f), "SetLevel"))
            {
                Player.SetLevel(uint.Parse(level) - 1);
            }

            if (GUI.Button(new Rect(100f, 100f, 200f, 200f), "D/E UI:"))
            {
                UILevel.Prefab.Instance.EnableOrDisableUI();
                UIPlayerMenu.Prefab.Instance.EnableOrDisableUI();
            }

            if (GUI.Button(new Rect(300f, 100, 200f, 200f), "D/E ParallaxElement: " + ParallaxBack.ShouldShow))
            {
                ParallaxBack.ShouldShow = !ParallaxBack.ShouldShow;

                ParallaxBack[] parallaxes = FindObjectsOfType<ParallaxBack>();

                foreach (ParallaxBack parallax in parallaxes)
                {
                    parallax.UpdateParallax();
                }
            }

            if (GUI.Button(new Rect(500f, 900f, 200f, 200f), "ShowRewarded"))
            {
                AdvertisingHelper.ShowVideo(null, AdPlacementType.DefaultPlacement);
            }

            if (GUI.Button(new Rect(700f, 900f, 200f, 200f), "ShowInterstitial"))
            {
                AdvertisingHelper.ShowInterstitial(AdPlacementType.DefaultPlacement);
            }
            
            if (GUI.Button(new Rect(100f, 1100f, 600f, 400f), "ShowMediationDebugger"))
            {
                LLMaxManager.ShowMediationDebugger();
            }
        }
        #endregion
    }
}
