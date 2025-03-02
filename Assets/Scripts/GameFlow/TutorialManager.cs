using Modules.General.HelperClasses;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
    {
        #region Types

        [Serializable]
        public class Data
        {
            public bool IsShootTutorialStarted;
            public bool IsShootTutorialPassed;
            public bool IsUpgradeWeaponTutorialPassed;
            public bool IsBuyWeaponTutorialPassed;
            public bool IsUpgradeAbilityTutorialPassed;
            public bool IsPrestigeTutorialPassed;
            public bool IsBuySkinTutorialPassed;
        }

        #endregion



        #region Variables

        private const string PREFS_KEY = "tutorials_prefs";

        public static event Action OnUpgradeTutorialPassed = delegate { };
        public static event Action OnUpgradeCharacterTutorialPassed = delegate { };
        public static event Action OnBuyTutorialStart = delegate { };
        public static event Action OnNeedTapToStart = delegate { };
        public static event Action<bool> OnLockShooter = delegate { };


        [SerializeField]
        private Image fadeImage = null;
        [SerializeField]
        private RectTransform tapImage = null;


        private Data data;

        #endregion



        #region Properties

        private bool IsShootTutorialStarted
        {
            get
            {
                return data.IsShootTutorialStarted;
            }
            set
            {
                data.IsShootTutorialStarted = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsShootTutorialPassed
        {
            get
            {
                return data.IsShootTutorialPassed;
            }
            set
            {
                data.IsShootTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsUpgradeWeaponTutorialPassed
        {
            get
            {
                return data.IsUpgradeWeaponTutorialPassed;
            }
            set
            {
                data.IsUpgradeWeaponTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsBuyWeaponTutorialPassed
        {
            get
            {
                return data.IsBuyWeaponTutorialPassed;
            }
            set
            {
                data.IsBuyWeaponTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsUpgradeAbilityTutorialPassed
        {
            get
            {
                return data.IsUpgradeAbilityTutorialPassed;
            }
            set
            {
                data.IsUpgradeAbilityTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);

                GameAnalytics.ShouldRepeatUpgradeCharacterTutorial = true;
                OnUpgradeCharacterTutorialPassed();
            }
        }


        public bool IsPrestigeTutorialPassed
        {
            get
            {
                return data.IsPrestigeTutorialPassed;
            }
            set
            {
                data.IsPrestigeTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsBuySkinTutorialPassed
        {
            get
            {
                return data.IsBuySkinTutorialPassed;
            }
            set
            {
                data.IsBuySkinTutorialPassed = value;
                CustomPlayerPrefs.SetObjectValue(PREFS_KEY, data);
            }
        }


        public bool IsShootTutorialCanShow
        {
            get
            {
                return !IsShootTutorialPassed && Player.Level == 0;
            }
        }


        public bool IsUpgradeWeaponTutorialCanStart
        {
            get
            {
                return !IsUpgradeWeaponTutorialPassed && IsShootTutorialPassed && Player.Level <= 1;
            }
        }


        public bool IsBuyTutorialCanStart
        {
            get
            {
                return !IsBuyWeaponTutorialPassed && IsUpgradeWeaponTutorialPassed && Player.GetWeaponsCount() == 1 && Player.Coins >= Arsenal.GetWeaponPrice(1);
            }
        }


        public bool IsUpgradeAbilityTutorialCanStart
        {
            get
            {
                return !IsUpgradeAbilityTutorialPassed && IsBuyWeaponTutorialPassed && Player.Coins >= PlayerConfig.GetBonusCoinsUpgradePrice(Player.BonusCoinsLevel);
            }
        }


        public bool IsPrestigeTutorialCanStart
        {
            get
            {
                return !IsPrestigeTutorialPassed && IsUpgradeAbilityTutorialPassed && PlayerConfig.IsResetAllow();
            }
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            data = CustomPlayerPrefs.GetObjectValue<Data>(PREFS_KEY);

            if (data == null)
            {
                data = new Data();
            }
        }

        private void Start()
        {
            if (Player.GetWeaponsCount() > 1)
            {
                IsBuyWeaponTutorialPassed = true;
            }

            if (Player.Level > 1)
            {
                IsShootTutorialStarted = true;
                IsShootTutorialPassed = true;
                IsUpgradeWeaponTutorialPassed = true;
            }

            if (IsShootTutorialCanShow)
            {
                Arena.OnStartLevel += OnStartLevel;
            }
        }

        #endregion



        #region Public methods

        public Data GetPrefs()
        {
            return data; 
        }


        public void UpdatePrefs(Data restoredData)
        {
            data.IsShootTutorialStarted |= restoredData.IsShootTutorialStarted;
            data.IsShootTutorialPassed |= restoredData.IsShootTutorialPassed;
            data.IsUpgradeWeaponTutorialPassed |= restoredData.IsUpgradeWeaponTutorialPassed;
            data.IsBuyWeaponTutorialPassed |= restoredData.IsBuyWeaponTutorialPassed;
            data.IsUpgradeAbilityTutorialPassed |= restoredData.IsUpgradeAbilityTutorialPassed;
            data.IsPrestigeTutorialPassed |= restoredData.IsPrestigeTutorialPassed;
            data.IsBuySkinTutorialPassed |= restoredData.IsBuySkinTutorialPassed;
        }


        public void SetFade(bool value)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.GetComponent<TweenImageColor>().Play(() => fadeImage.gameObject.SetActive(value), value);
        }


        public void ShowTapToStart()
        {
            OnNeedTapToStart();
        }


        public void TrueWeaponUpgradeTutorialPassed()
        {
            OnUpgradeTutorialPassed();
            GameAnalytics.ShouldRepeatUpgradeTutorial = true;
        }

        #endregion



        #region Private methods

        private void OnStartLevel()
        {
            if (!IsShootTutorialStarted)
            {
                GameAnalytics.StartFirstTutorialEvent();
                IsShootTutorialStarted = true;
            }

            OnLockShooter(true);
            StartCoroutine(ShootTutorial());

            Arena.OnStartLevel -= OnStartLevel;
        }


        private IEnumerator ShootTutorial()
        {
            yield return new WaitForSeconds(1f);

            SetFade(true);
            tapImage.gameObject.SetActive(true);


            OnLockShooter(false);
            TapZone.OnTap += OnTap;
        }


        private void OnTap()
        {
            SetFade(false);
            tapImage.gameObject.SetActive(false);

            TapZone.OnTap -= OnTap;
            Player.OnLevelUp += OnLevelUp;
        }


        private void OnLevelUp()
        {
            if (!IsShootTutorialPassed)
            {
                IsShootTutorialPassed = true;
            }
        }

        #endregion
    }
}
