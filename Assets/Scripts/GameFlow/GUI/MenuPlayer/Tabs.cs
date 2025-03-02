using MoreMountains.NiceVibrations;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class Tabs : MonoBehaviour
    {
        #region Types

        [Serializable]
        private class Tab
        {
            public Button Button;
            public MenuPlayerPanel Panel;
            public RectTransform Alert = null;
        }

        #endregion



        #region Variables

        [SerializeField]
        private Tab[] tabs = null;
        [SerializeField]
        private int defaultTapAfterShow = 0;

        [Header("Vibration")]
        [SerializeField]
        private HapticTypes buttonClickVibrationType = HapticTypes.None;


        private Tab selectedTab;
        private float currentCoins;
        private float currentGems;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            TutorialManager.OnBuyTutorialStart += OnBuyTutorialStart;
            Player.OnChangeCoins += RefreshAlert;
            Player.OnChangeGems += RefreshAlert;

            foreach (var tab in tabs)
            {
                tab.Button.onClick.AddListener(() => SelectTab(tab));
            }

            SelectTab(tabs[defaultTapAfterShow > -1 ? Mathf.Clamp(defaultTapAfterShow, 0, tabs.Length - 1) : 0]);
        }


        private void OnEnable()
        {
            if (defaultTapAfterShow > -1)
            {
                SelectTab(tabs[Mathf.Clamp(defaultTapAfterShow, 0, tabs.Length - 1)]);
            }
        }


        private void OnDestroy()
        {
            TutorialManager.OnBuyTutorialStart -= OnBuyTutorialStart;
            Player.OnChangeCoins -= RefreshAlert;
            Player.OnChangeGems -= RefreshAlert;
        }

        #endregion



        #region Public methods

        public void SetAlert()
        {
            foreach (var tab in tabs)
            {
                tab.Alert.gameObject.SetActive(tab.Panel.NeedShowAlert());
            }

            selectedTab.Alert.gameObject.SetActive(false);
        }

        #endregion



        #region Private methods

        private void RefreshAlert()
        {
            foreach (var tab in tabs)
            {
                if (!tab.Panel.NeedShowAlert())
                {
                    tab.Alert.gameObject.SetActive(false);
                }
            }
        }

        private void SelectTab(Tab clickTab)
        {
            if (selectedTab == clickTab)
            {
                return;
            }

            selectedTab = clickTab;

            foreach (var tab in tabs)
            {
                tab.Button.GetComponent<MultiImageButton>().Interactable(true);
                tab.Panel.gameObject.SetActive(false);
            }

            clickTab.Button.GetComponent<MultiImageButton>().Interactable(false);
            clickTab.Panel.gameObject.SetActive(true);
            clickTab.Alert.gameObject.SetActive(false);

            clickTab.Button.transform.SetAsLastSibling();

            VibrationManager.Instance.PlayVibration(buttonClickVibrationType);
        }

        private void OnBuyTutorialStart()
        {
            SelectTab(tabs[0]);
        }

        #endregion

    }
}
