using Modules.Advertising;
using Modules.Analytics;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UILevel : UITweensUnit<UnitResult>
    {
        [System.Serializable]
        public class BoosterTimerInfo
        {
            public GUIControl horizontalContol;
            public GUIControl verticalContol;
            public Image progressBarImage;
            public Image progressBarHeadImage;
            [System.NonSerialized]
            public float durationLeft;
        }


        #region Variables

        public static readonly ResourceGameObject<UILevel> Prefab = new ResourceGameObject<UILevel>("Game/GUI/PanelArena");

        [Header("Content")]
        [SerializeField]
        private RectTransform body = null;
        [SerializeField]
        private Image imageHealth = null;
        [SerializeField]
        private Text textAmmo = null;
        [SerializeField]
        private TextMeshProUGUI textCoins = null;
        [SerializeField]
        private Text textLevel = null;
        [SerializeField]
        private Text textNextLevel = null;
        [SerializeField]
        private Image backNextLevel = null;

        [Header("Boosters")]
        [SerializeField]
        private BoosterTimerInfo x2Coins = null;
        [SerializeField]
        private BoosterTimerInfo x2Booster = null;
        [SerializeField]
        private float progressBarRadius = 70.5f;

        [SerializeField]
        private Button settingsButton = null;

        public bool shouldShow = true;

        #endregion



        #region Properties

        private Vector2 SaveTopOffset { get; set; }

        private Vector2 SaveTopWithBannerOffset { get; set; }
        
        public DateTime StartLevelTime { get; private set; }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            Rect canvasRect = GetComponent<RectTransform>().rect;
            SaveTopOffset = new Vector2(body.anchorMax.x, body.anchorMax.y - SafeOffset.GetSafeTopRatio(canvasRect));
            SaveTopWithBannerOffset = new Vector2(body.anchorMax.x, body.anchorMax.y - SafeOffset.GetSafeTopWithBannerRatio(canvasRect));
            body.anchorMax = SaveTopOffset;
            CustomAdvertisingManager.OnBannerVisibilityChanged += CustomAdvertisingManagerOnBannerVisibilityChanged;
            settingsButton.onClick.AddListener(SettingsButton_OnClick);
        }

        
        private void Update()
        {
            float coinsAmount = CoinsBooster.asset.Value.PercentOfBoosterDurationLeft;
            float boosterAmount = ShooterUpgradesBooster.asset.Value.PercentOfBoosterDurationLeft;
            x2Coins.durationLeft = CoinsBooster.asset.Value.BoosterDurationLeft;
            x2Booster.durationLeft = ShooterUpgradesBooster.asset.Value.BoosterDurationLeft;

            if (coinsAmount > float.Epsilon)
            {
                SetProgressBarState(coinsAmount, x2Coins);
            }
            if (x2Coins.durationLeft > float.Epsilon)
            {
                TryShowBooster(x2Coins, x2Booster);
            }
            else
            {
                TryHideBooster(x2Coins, x2Booster);
            }
            
            if (boosterAmount > float.Epsilon)
            {
                SetProgressBarState(boosterAmount, x2Booster);
            }
            
            if (x2Booster.durationLeft > float.Epsilon)
            {
                TryShowBooster(x2Booster, x2Coins);
            }
            else
            {
                TryHideBooster(x2Booster, x2Coins);
            }
        }


        private void OnDestroy()
        {
            CustomAdvertisingManager.OnBannerVisibilityChanged -= CustomAdvertisingManagerOnBannerVisibilityChanged;
            settingsButton.onClick.RemoveListener(SettingsButton_OnClick);
        }

        #endregion



        #region Public methods

        public override void Show(System.Action<UnitResult> onHided = null, System.Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            CustomAdvertisingManagerOnBannerVisibilityChanged(
                (CustomAdvertisingManager.Instance as AdvertisingManager).IsBannerShowing);
            
            StartLevelTime = DateTime.Now;
        }


        public void Coins(float value)
        {
            textCoins.text = value.ToShortFormat();
        }


        public void Ammo(uint value)
        {
            textAmmo.text = value.ToString();
        }


        public void HealthPinata(float value)
        {
            imageHealth.fillAmount = 1f - value;
            backNextLevel.enabled = (value < 0f);
        }


        public void Level(uint value)
        {
            textLevel.text = value.ToString();
            textNextLevel.text = (value + 1u).ToString();
        }


        public void EnableOrDisableUI()
        {
            shouldShow = !shouldShow;
            body.gameObject.SetActive(shouldShow);
        }

        #endregion



        #region Private methods

        private void SetProgressBarState(float amount, BoosterTimerInfo booster)
        {
            amount = Mathf.Clamp01(amount);
            booster.progressBarImage.fillAmount = amount;
            booster.progressBarHeadImage.transform.localPosition = new Vector3(-Mathf.Sin(amount * 6.28f), -Mathf.Cos(amount * 6.28f), 0f) * progressBarRadius;
        }


        private void TryShowBooster(BoosterTimerInfo targetBooster, BoosterTimerInfo otherBooster)
        {
            if (targetBooster.horizontalContol.IsHidden)
            {
                bool isLessBooster = otherBooster.durationLeft > targetBooster.durationLeft;
                if (otherBooster.horizontalContol.IsShown || isLessBooster)
                {
                    if (isLessBooster)
                    {
                        targetBooster.verticalContol.ShowImmediately();
                    }
                    else
                    {
                        targetBooster.verticalContol.HideImmediately();
                        otherBooster.verticalContol.Show();
                    }
                }
                else
                {
                    targetBooster.verticalContol.HideImmediately();
                }
                
                targetBooster.horizontalContol.Show();
            }
        }


        private void TryHideBooster(BoosterTimerInfo targetBooster, BoosterTimerInfo otherBooster)
        {
            if (targetBooster.horizontalContol.IsShown)
            {
                targetBooster.horizontalContol.Hide();
                if (otherBooster.verticalContol.IsShown)
                {
                    otherBooster.verticalContol.Hide();
                }
            }
        }

        #endregion



        #region Events Handler
        
        private void CustomAdvertisingManagerOnBannerVisibilityChanged(bool isVisible)
        {
            body.anchorMax = (isVisible) ? (SaveTopWithBannerOffset) : (SaveTopOffset);
        }


        private void SettingsButton_OnClick() 
        {
                TapZone.Lock(true);

                UISettings.Prefab.Instance.Show((_) =>
                {
                    TapZone.Lock(false);
                });
        }

        #endregion
    }
}
