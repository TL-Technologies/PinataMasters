using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIRateUs : UITweensUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UIRateUs> Prefab = new ResourceGameObject<UIRateUs>("Game/GUI/DialogRateUs");

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Button buttonRate = null;
        [SerializeField]
        private Button buttonClose = null;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonRate.onClick.AddListener(Rate);
            buttonClose.onClick.AddListener(Close);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);
            tweenColor.Duration = durationShow;
            tweenColor.Play();

            RateUs.SetAsShowed();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }

        #endregion



        #region Private methods

        private void Rate()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else
            {
                Application.OpenURL(RateUs.RateUsURL);
                RateUs.SetAsRated();
                Hide();
            }
        }


        private void Close()
        {
            Hide();
        }

        #endregion
    }
}
