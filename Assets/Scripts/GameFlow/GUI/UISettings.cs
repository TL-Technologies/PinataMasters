using Modules.Advertising;
using Modules.General.Abstraction;
using MoreMountains.NiceVibrations;
using System;
using Modules.General;
using Modules.Networking;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UISettings : UITweensUnit<UnitResult>
    {
        #region Variables

        private const string SOUND_ON_TEXT_KEY = "ui.settings.soundon";
        private const string SOUND_OFF_TEXT_KEY = "ui.settings.soundoff";

        private const string MUSIC_ON_TEXT_KEY = "ui.settings.musicon";
        private const string MUSIC_OFF_TEXT_KEY = "ui.settings.musicoff";

        private const string VIBRATION_ON_TEXT_KEY = "ui.settings.vibrationon";
        private const string VIBRATION_OFF_TEXT_KEY = "ui.settings.vibrationoff";

        private const string AdLockReason = "facebookLogin";

        public static readonly ResourceGameObject<UISettings> Prefab = new ResourceGameObject<UISettings>("Game/GUI/DialogSettings");

        public static event Action OnHide;

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Button buttonClose = null;
        [Space]
        [SerializeField]
        private Button buttonSound = null;
        [SerializeField]
        private TextLocalizator buttonSoundText = null;
        [Space]
        [SerializeField]
        private Button buttonMusic = null;
        [SerializeField]
        private TextLocalizator buttonMusicText = null;
        [Space]
        [SerializeField]
        private Button buttonVibration = null;
        [SerializeField]
        private TextLocalizator buttonVibrationText = null;
        [Space]
        [SerializeField]
        private Button buttonGDPR = null;
        [SerializeField]
        private Button buttonLanguage = null;
        [SerializeField]
        private Button buttonRestore = null;
        [Header("Facebook")]
        [SerializeField]
        private Button buttonFacebookLogin = null;
        [SerializeField]
        private GameObject textFacebookLogin = null;
        [SerializeField]
        private GameObject textFacebookLogined = null;
        [SerializeField]
        private GameObject textFacebookSynchronization = null;
        [SerializeField]
        private Sprite spriteFacebookLogin = null;
        [SerializeField]
        private Sprite spriteFacebookLogined = null;

        #endregion



        #region Properties

        public bool IsShowing
        {
            get;
            private set;
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonSound.onClick.AddListener(Sound);
            buttonMusic.onClick.AddListener(Music);
            buttonVibration.onClick.AddListener(Vibrations);
            buttonClose.onClick.AddListener(Close);
            buttonLanguage.onClick.AddListener(SetNextLanguage);
            buttonRestore.onClick.AddListener(TryRestore);
            buttonGDPR.onClick.AddListener(TermsAndPolicy);
            buttonFacebookLogin.onClick.AddListener(TryLoginFacebook);
            buttonGDPR.gameObject.SetActive(Services.GetService<IPrivacyManager>().IsPrivacyButtonAvailable);

            #if UNITY_IOS
                buttonRestore.gameObject.SetActive(true);
            #elif UNITY_ANDROID
                buttonRestore.gameObject.SetActive(false);
            #endif
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            tweenColor.Duration = durationShow;
            tweenColor.Play();

            RefreshText();

            IsShowing = true;
            
            AdvertisingHelper.ShowInterstitial(AdPlacementType.SettingsOpen);
        }


        public void Hide()
        {
            base.Hide();

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);

            IsShowing = false;

            OnHide?.Invoke();
            
            AdvertisingHelper.ShowInterstitial(AdPlacementType.SettingsClose);
        }


        public void RefreshText()
        {
            string key = AudioManager.Instance.IsSoundEnable ? SOUND_ON_TEXT_KEY : SOUND_OFF_TEXT_KEY;
            buttonSoundText.SetKey(key);

            key = AudioManager.Instance.IsMusicEnable ? MUSIC_ON_TEXT_KEY : MUSIC_OFF_TEXT_KEY;
            buttonMusicText.SetKey(key);

            key = VibrationManager.Instance.IsVibrationEnabled ? VIBRATION_ON_TEXT_KEY : VIBRATION_OFF_TEXT_KEY;
            buttonVibrationText.SetKey(key);


            // #if UNITY_IOS
            //     buttonFacebookLogin.gameObject.SetActive(true);
            //     bool loggedIn = CloudProgress.IsUserLoggedIn;
            //     bool isButtonInteractable = !loggedIn || (loggedIn && !CloudProgress.IsSynchronizeEnabled);
            //     buttonFacebookLogin.interactable = isButtonInteractable;
            //     textFacebookLogin.SetActive(!loggedIn);
            //     textFacebookLogined.SetActive(loggedIn && CloudProgress.IsSynchronizeEnabled);
            //     textFacebookSynchronization.SetActive(loggedIn && !CloudProgress.IsSynchronizeEnabled);
            //     buttonFacebookLogin.GetComponent<Image>().sprite = !isButtonInteractable ? spriteFacebookLogined : spriteFacebookLogin;
            // #else
                buttonFacebookLogin.gameObject.SetActive(false);
            // #endif
        }

        #endregion



        #region Private methods

        private void Close()
        {
            Hide();
        }


        private void Sound()
        {
            AudioManager.Instance.IsSoundEnable = !AudioManager.Instance.IsSoundEnable;
            RefreshText();
        }


        private void Music()
        {
            AudioManager.Instance.IsMusicEnable = !AudioManager.Instance.IsMusicEnable;
            RefreshText();
        }


        private void Vibrations()
        {
            VibrationManager.Instance.IsVibrationEnabled = !VibrationManager.Instance.IsVibrationEnabled;
            RefreshText();
        }


        private void TermsAndPolicy()
        {
            GameAnalytics.SendGDPRSettingClickEvent();

            if (ReachabilityHandler.Instance.NetworkStatus == NetworkStatus.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
                return;
            }

            EventSystemController.DisableEventSystem();

            Services.GetService<IPrivacyManager>().GetTermsAndPolicyURI((bool success, string url) =>
            {
                EventSystemController.EnableEventSystem();

                if (success)
                {
                    Application.OpenURL(url);
                }
                else
                {
                    UIInfo.Prefab.Instance.Show(UIInfo.Type.SomethingWrong);
                }
            });
        }


        private void SetNextLanguage()
        {
            Localisation.SetNextLanguage();
        }


        private void TryLoginFacebook()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
                return;
            }

            if (CloudProgress.IsUserLoggedIn && !CloudProgress.IsSynchronizeEnabled)
            {
                CloudProgress.IsSynchronizeEnabled = true;
                CloudProgress.TryShowRestoreProgress();
                RefreshText();
            }
            else
            {
                buttonFacebookLogin.interactable = false;
                AdvertisingManager.Instance.LockAd(AdModule.Interstitial, AdLockReason);
                CloudProgress.LoginFacebook(FacebookLoginCallback);
            }
        }


        private void FacebookLoginCallback()
        {
            AdvertisingManager.Instance.UnlockAd(AdModule.Interstitial, AdLockReason);
            RefreshText();
        }


        private void TryRestore()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
                return;
            }

            GameAnalytics.SendTryRestoreEvent();
            EventSystemController.DisableEventSystem();
            UILoader.Prefab.Instance.Show();

            IAPs.RestorePurchase((success) =>
            {
                UILoader.Prefab.Instance.Hide();
                EventSystemController.EnableEventSystem();
                if (success)
                {
                    UIInfo.Prefab.Instance.Show(UIInfo.Type.Restore);
                    if (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive)
                    {
                        GameAnalytics.SendDoneRestoreEvent();
                        AdvertisingHelper.ShowBanner();
                        Hide();
                    }
                }
            });
        }

        #endregion
    }
}
