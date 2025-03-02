using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UISocialPopUp : UITweensUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UISocialPopUp> Prefab = new ResourceGameObject<UISocialPopUp>("Game/GUI/DialogSocial");

        private const string RED_HEAR_SPRITE = "<sprite=0>";

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private GameObject[] subscribe = null;
        [SerializeField]
        private GameObject[] like = null;

        [SerializeField]
        private Button buttonSubscribe = null;
        [SerializeField]
        private Button buttonLike = null;

        [SerializeField]
        private Text textCoins = null;

        [SerializeField]
        private Button closeBottun = null;

        [SerializeField]
        private TextMeshProLocalizator textLikeDesc = null;

        private string urlLink;
        private LLPromoType type;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonSubscribe.onClick.AddListener(ClaimBonus);
            buttonLike.onClick.AddListener(ClaimBonus);
            closeBottun.onClick.AddListener(ClosePopUp);
        }

        #endregion



        #region Public methods

        public void Show(LLPromoFetcherUnit unit)
        {
            urlLink = unit.promoURL;
            type = unit.promoType;
            foreach (var i in like)
            {
                i.SetActive(unit.promoType == LLPromoType.Like);
                textLikeDesc.SetParams(RED_HEAR_SPRITE, RED_HEAR_SPRITE);
            }

            foreach (var i in subscribe)
            {
                i.SetActive(unit.promoType == LLPromoType.Subscription);
            }

            textCoins.text = "+" + SocialController.GetCurrentCoins().ToShortFormat();

            Show();
        }


        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided,onShowed);

            tweenColor.Duration = durationShow;
            tweenColor.Play();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }

        #endregion



        #region Private methods

        private void ClaimBonus()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else
            {
                Player.AddCoins((uint)SocialController.GetCurrentCoins());
                SocialController.TakeBonus();
                Application.OpenURL(urlLink);
            }

            Hide();
        }


        private void ClosePopUp()
        {
            SocialController.SkipBonus();
            Hide();
        }

        #endregion
    }
}
