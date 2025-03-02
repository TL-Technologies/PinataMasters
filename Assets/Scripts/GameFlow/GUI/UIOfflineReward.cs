using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIOfflineReward : UITweensUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UIOfflineReward> Prefab = new ResourceGameObject<UIOfflineReward>("Game/GUI/DialogOfflineReward");

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [SerializeField]
        private AudioClip showClip = null;

        [Header("Content")]
        [SerializeField]
        private Button buttonCollect = null;
        [SerializeField]
        private Text textButtonCollect = null;

        [SerializeField]
        private Button buttonCollectBonus = null;
        [SerializeField]
        private Text textButtonCollectBonus = null;

        [SerializeField]
        private GameObject redSimpleLabel = null;

        [SerializeField]
        private GameObject purpleBonusLabel = null;

        [SerializeField]
        private Text coinsMultiplierForAdsText = null; 

        private bool isSubscriptionActive;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonCollect.onClick.AddListener(Collect);
            buttonCollectBonus.onClick.AddListener(CollectBonus);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            isSubscriptionActive = IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive;

            textButtonCollect.text = (OfflineReward.OfflineCoinsBank * OfflineReward.GetSimpleMultiplier(isSubscriptionActive)).ToShortFormat();
            textButtonCollectBonus.text = (OfflineReward.OfflineCoinsBank * OfflineReward.GetBonusMultiplier(isSubscriptionActive)).ToShortFormat();

            coinsMultiplierForAdsText.text = String.Format("X{0}", OfflineReward.GetBonusMultiplier(isSubscriptionActive));

            redSimpleLabel.SetActive(true);
            purpleBonusLabel.SetActive(isSubscriptionActive);

            tweenColor.Duration = durationShow;
            tweenColor.Play();

            AudioManager.Instance.Play(showClip, AudioType.Sound);
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            OfflineReward.ResetOfflineCoinsBank();
            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }

        #endregion



        #region Private methods

        private void Collect()
        {
            Player.AddCoins(OfflineReward.TakeOfflineReward() * OfflineReward.GetSimpleMultiplier(isSubscriptionActive));
            Hide();
        }


        private void CollectBonus()
        {
            float reward = OfflineReward.TakeOfflineReward() * OfflineReward.GetBonusMultiplier(isSubscriptionActive);
            
            AdvertisingHelper.ShowVideo((result) =>
            {
                if (result)
                {
                    Player.AddCoins(reward);
                    Hide();
                }
            }, CustomAdPlacementType.OfflineReward, reward.ToString());
        }

        #endregion
    }
}
