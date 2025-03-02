using Modules.General;
using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace Modules.Advertising
{   
    public class AdvertisingEditorCanvas : MonoBehaviour
    {
        #region Nested types

        public class ResourceAsset<TObject> where TObject : Object
        {
            readonly string path;
            TObject asset;

            public ResourceAsset(string path)
            {
                this.path = path;
            }

            public TObject Value
            {
                get
                {
                    asset = asset ?? Resources.Load<TObject>(path);
                    return asset as TObject;
                }
            }
        }


        public class ResourceGameObject<TMonoBehaviour> : ResourceAsset<GameObject> where TMonoBehaviour : MonoBehaviour
        {
            GameObject instance;

            public TMonoBehaviour Instance
            {
                get
                {
                    Instantiate();
                    return instance.GetComponent<TMonoBehaviour>();
                }
            }

            public ResourceGameObject(string path) : base(path) { }

            public void Instantiate()
            {
                instance = instance ? instance : Object.Instantiate(Value);
            }
        }

        #endregion



        #region Fields

        public static readonly ResourceGameObject<AdvertisingEditorCanvas> Prefab = new ResourceGameObject<AdvertisingEditorCanvas>("AdvertisingEditorCanvas");
        public static Action<AdModule, AdActionResultType> OnEditorAdHide; 

        [Header("Content")]
        [SerializeField] private Button closeButton = null;
        [SerializeField] private BannerView bannerView = null;
        [SerializeField] private GameObject fullScreenAdView = null;
        [SerializeField] private Text fullScreenAdText = null;
        [SerializeField] private Text timerText = null;
        [SerializeField] private Text placementText = default;
        [SerializeField] private EventSystem eventSystemPrefab = null;
        [SerializeField] private CanvasScaler canvasScaler = null;

        private SimpleTimer timer = new SimpleTimer(0.0f, null);
        private SimpleTimer interstitialCloseButtonTimer = new SimpleTimer(0.0f, null);
        private AdModule currentModule = AdModule.None;
        private Dictionary<AdModule, List<AdModuleSettings.AdCloseStatusHotKey>> moduleHotKeys = new Dictionary<AdModule, List<AdModuleSettings.AdCloseStatusHotKey>>();

        private bool shouldTimersUseUnscaledDeltaTime = false;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            fullScreenAdView.SetActive(false);
            bannerView.gameObject.SetActive(false);

            if (EventSystem.current == null)
            {
                Instantiate(eventSystemPrefab, transform);
            }

            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }


        private void Update()
        {
            float time = (shouldTimersUseUnscaledDeltaTime) ? (Time.unscaledDeltaTime) : (Time.deltaTime);

            interstitialCloseButtonTimer.CustomUpdate(time);
            timer.CustomUpdate(time);
            timerText.text = Mathf.CeilToInt(timer.RemainingTime).ToString();

            CheckHotkeys();
        }


        private void CheckHotkeys()
        {
            if (!moduleHotKeys.ContainsKey(currentModule))
            {
                return;
            }

            for (int i = 0; i < moduleHotKeys[currentModule].Count; i++)
            {
                AdModuleSettings.AdCloseStatusHotKey hotkeyData = moduleHotKeys[currentModule][i];
                if (Input.GetKeyDown(hotkeyData.hotkey))
                {
                    closeButton.onClick.RemoveListener(CloseButton_OnClick);
                    HideAdView(currentModule, hotkeyData.closeResultType);
                    timer.ResetTimer(0.0f, null);
                }
            }
        }


        void OnEnable()
        {
            closeButton.onClick.AddListener(CloseButton_OnClick);
        }


        void OnDisable()
        {
            closeButton.onClick.RemoveListener(CloseButton_OnClick);
        }

        #endregion



        #region Methods

        public void ShowBanner(
            string placementName,
            CustomBannerSettings bannerSettings, 
            Action<AdActionResultType, int, string, string> showCallback)
        { 
            currentModule = AdModule.Banner;
            moduleHotKeys[currentModule] = bannerSettings.HotKeys;

            bannerView.Show(bannerSettings, placementName);

            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                showCallback?.Invoke(AdActionResultType.Success, 0, null, "adIdentifier");
            }, 0.0f);
        }


        public void ShowInterstitial(
            string placementName,
            VideoSettings videoSettings,
            InterstitialSettings moduleSettings,
            Action<AdActionResultType, int, string, string> showCallback)
        {
            currentModule = AdModule.Interstitial;
            moduleHotKeys[currentModule] = moduleSettings.HotKeys;

            ChangeCloseButtonActivity(false);
            shouldTimersUseUnscaledDeltaTime = videoSettings.ShouldUseUnscaledDeltaTime;
            timer.ResetTimer(videoSettings.FullScreenAdDuration, null);
            interstitialCloseButtonTimer.ResetTimer(videoSettings.InterstitialCloseButtonDelay, 
                () => ChangeCloseButtonActivity(true));
            fullScreenAdView.SetActive(true);
            fullScreenAdText.text = currentModule.ToString();
            placementText.text = placementName;

            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                showCallback?.Invoke(AdActionResultType.Success, 0, null, "adIdentifier");
            }, 0.0f);
        }
        
        
        public void ShowRewardedVideo(
            string placementName, 
            VideoSettings videoSettings,
            RewardedVideoSettings moduleSettings,
            Action<AdActionResultType, int, string, string> showCallback)
        { 
            currentModule = AdModule.RewardedVideo;
            moduleHotKeys[currentModule] = moduleSettings.HotKeys;
            
            ChangeCloseButtonActivity(true);
            shouldTimersUseUnscaledDeltaTime = videoSettings.ShouldUseUnscaledDeltaTime;
            timer.ResetTimer(videoSettings.FullScreenAdDuration, null);
            fullScreenAdView.SetActive(true);
            fullScreenAdText.text = currentModule.ToString();
            placementText.text = placementName;
   
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                showCallback?.Invoke(AdActionResultType.Success, 0, null, "adIdentifier");
            }, 0.0f);
        }


        public void HideAdView(AdModule module, AdActionResultType resultStatus)
        {
            switch (module)
            {
                case AdModule.Banner:
                    bannerView.Hide();
                    Scheduler.Instance.CallMethodWithDelay(this,
                        () => { OnEditorAdHide?.Invoke(module, resultStatus); }, 0.0f);
                    break;

                case AdModule.Interstitial:
                case AdModule.RewardedVideo:
                    fullScreenAdView.SetActive(false);
                    Scheduler.Instance.CallMethodWithDelay(this,
                        () => { OnEditorAdHide?.Invoke(module, resultStatus); }, 0.0f);
                    break;

                default:
                    CustomDebug.LogError("Try to hide advertising with unknown type!");
                    break;
            }
        }


        private void ChangeCloseButtonActivity(bool isActive)
        {
            closeButton.gameObject.SetActive(isActive);
        }

        #endregion



        #region Events handlers

        private void CloseButton_OnClick()
        {
            HideAdView(currentModule, timer.RemainingTime <= 0.0f ? AdActionResultType.Success : AdActionResultType.Skip);
            timer.ResetTimer(0.0f, null);
        }

        #endregion
    }
}
