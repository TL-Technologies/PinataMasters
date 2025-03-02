using Modules.Advertising;
using Modules.General;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIOfferDialog : UITweensUnit<UIOfferDialog.Result>
    {

        #region Nested types

        public class Result : UnitResult
        {
            public bool IsSuccessfully;
        }


        [Serializable]
        public struct Content
        {
            public IngameOfferRewardSettings contentRewardSettings;
            public UIOfferContentDialog contentPrefab;
            public GameObject buttonContent;
            public Text rewardText;
        }

        #endregion



        #region Fields

        public static readonly ResourceGameObject<UIOfferDialog> Prefab = new ResourceGameObject<UIOfferDialog>("Game/GUI/Offers/OfferDialog");

        public static event Action<bool> OnActiveStateChanged;

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField] private Transform contentRoot;
        [SerializeField] private List<Content> contentsByType;
        [SerializeField] private Button buttonGet;
        [SerializeField] private Button buttonClose;

        UIOfferContentDialog dialogContent;

        IngameOfferType currentOfferType;

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

            buttonGet.onClick.AddListener(OnButtonGetClick);
            buttonClose.onClick.AddListener(OnButtonCloseClick);
        }

        #endregion



        #region Public methods

        public void Show(IngameOfferRewardSettings settings, Action<Result> onHided, Action onShowed = null)
        {
            void hideCallback(Result result)
            {
                if (dialogContent != null)
                {
                    Destroy(dialogContent.gameObject);
                }

                OnActiveStateChanged?.Invoke(false);

                onHided?.Invoke(result);
            }


            InitializeContent(settings);

            Show(hideCallback, onShowed);

            currentOfferType = settings.OfferType;
            IsShowing = true;

            OnActiveStateChanged?.Invoke(true);

            tweenColor.Duration = durationShow;
            tweenColor.Play();
        }               

        #endregion



        #region Private methods

        private void InitializeContent(IngameOfferRewardSettings settings)
        {
            Content popupContent = contentsByType.Find((content) => content.contentRewardSettings == settings);

            dialogContent = Instantiate(popupContent.contentPrefab);
            dialogContent.transform.SetParent(contentRoot);

            for (int i = 0; i < contentsByType.Count; i++)
            {
                bool isButtonEnabled = (contentsByType[i].contentRewardSettings == settings);
                contentsByType[i].buttonContent.SetActive(isButtonEnabled);

                if (isButtonEnabled)
                {
                    int buttonIndex = i; // To avoid exceptions with Scheduler
                    Scheduler.Instance.CallMethodWithDelay(this, () => // To recalculate layout when language changed
                    {
                        LayoutRebuilder.MarkLayoutForRebuild(contentsByType[buttonIndex].buttonContent.gameObject.GetComponent<RectTransform>());
                    }, 0.03f);
                }
            }


            if (popupContent.rewardText != null)
            {
                popupContent.rewardText.text = settings.Value.ToShortFormat(true);
            }
            RectTransform contentRectTransform = dialogContent.GetComponent<RectTransform>();

            contentRectTransform.localPosition = Vector3.zero;
            contentRectTransform.localScale = Vector3.one;

            contentRectTransform.anchorMin = Vector2.zero;
            contentRectTransform.anchorMax = Vector2.one;

            contentRectTransform.sizeDelta = Vector2.zero;
        }


        private void OnButtonGetClick()
        {
            string placement = CustomAdPlacementType.GetIngameOfferPlacement(currentOfferType);
            
            GameAnalytics.IngameOfferTry(placement);

            AdvertisingHelper.ShowVideo((result) =>
            {
                if (result)
                {
                    IsShowing = false;
                    GameAnalytics.IngameOfferDone(placement);
                    Hide(new Result { IsSuccessfully = true });
                }
            }, placement);
        }
        

        private void OnButtonCloseClick()
        {
            IsShowing = false;
            Hide(new Result { IsSuccessfully = false });
            GameAnalytics.IngameOfferSkip(CustomAdPlacementType.GetIngameOfferPlacement(currentOfferType));
        }

        #endregion

    }
}
