using Modules.General.Abstraction;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UILevelResult : UITweensUnit<UnitResult>
    {
        #region Variables

        private const uint SIMPLE_MULTIPLIER = 1u;
        private const uint SIMPLE_SUBSCRIPTION_MULTIPLIER = 2u;

        private const uint BONUS_MULTIPLIER = 2u;
        private const uint BONUS_SUBSCRIPTION_MULTIPLIER = 4u;

        public static readonly ResourceGameObject<UILevelResult> Prefab = new ResourceGameObject<UILevelResult>("Game/GUI/DialogArenaResult");

        [SerializeField]
        private AudioClip showClip = null;

        [Header("Content")]
        [SerializeField]
        private Button buttonCollectBig = null;
        [SerializeField]
        private Button buttonCollect = null;
        [SerializeField]
        private Button buttonCollectBonus = null;
        [SerializeField]
        private GameObject oneButtonLayout = null;
        [SerializeField]
        private Text textCoinsBig = null;
        [SerializeField]
        private Text textCoins = null;
        [SerializeField]
        private Text textBonusCoins = null;
        [SerializeField]
        private GameObject[] objectsBonusCoins = null;
        [SerializeField]
        private Text textButtonCollect = null;
        [SerializeField]
        private Text textButtonCollectBonus = null;

        [SerializeField]
        private GameObject redSimpleLabel = null;
        [SerializeField]
        private GameObject redBonusLabel = null;
        [SerializeField]
        private GameObject purpleBonusLabel = null;
        [SerializeField]
        private GameObject purpleBonusLabelBig = null;

        private bool isSubscriptionActive;
        private Arena.Result arenaResult;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            buttonCollect.onClick.AddListener(Collect);
            buttonCollectBig.onClick.AddListener(Collect);
            buttonCollectBonus.onClick.AddListener(CollectBonus);
        }

        #endregion



        #region Public methods

        public void Show(Arena.Result result, Action<UnitResult> onHided)
        {
            isSubscriptionActive = IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive;
            arenaResult = result;
            arenaResult.Coins = result.Coins * CoinsBooster.asset.Value.CoinsMultiplier * Player.GetBonusCoinsSkill();
            textCoinsBig.text = (PlayerConfig.GetResultCoins(arenaResult.Coins) * GetSimpleResultMultiplier()).ToShortFormat();
            textButtonCollect.text = (PlayerConfig.GetResultCoins(arenaResult.Coins) * GetSimpleResultMultiplier()).ToShortFormat();
            textButtonCollectBonus.text = (PlayerConfig.GetResultCoins(arenaResult.Coins) * GetBonusResultMultiplier()).ToShortFormat();

            float totalEarnedCoins = PlayerConfig.GetResultCoins(arenaResult.Coins);
            float bonusCoins = totalEarnedCoins * (1.0f - (1.0f / (1.0f + PlayerConfig.GetBonusCoins(Player.BonusCoinsLevel))));
            textCoins.text = (totalEarnedCoins - bonusCoins).ToShortFormat();
            textBonusCoins.text = bonusCoins.ToShortFormat();
            foreach (GameObject obj in objectsBonusCoins)
            {
                obj.SetActive(bonusCoins > float.Epsilon);
            }

            if (Player.Level < 2)
            {
                buttonCollectBonus.gameObject.SetActive(false);
                buttonCollect.gameObject.SetActive(false);
                redSimpleLabel.SetActive(false);
                redBonusLabel.SetActive(false);
                purpleBonusLabel.SetActive(false);
                purpleBonusLabelBig.SetActive(isSubscriptionActive);
                buttonCollectBig.gameObject.SetActive(true);
                oneButtonLayout.SetActive(true);
            }
            else
            {
                long levelsWinsCount = Player.Level - 1;

                int x2OfferLossFrequency = ABTest.InGameAbTestData.x2VideoFrequencyLoss;
                int x2OfferWinFrequency = ABTest.InGameAbTestData.x2VideoFrequencyWin;

                bool shouldShowBonusButton =  (result.Win && (levelsWinsCount != 0) && (x2OfferWinFrequency != 0) && (levelsWinsCount % x2OfferWinFrequency == 0)) ||
                                              (!result.Win && (LevelLosses.Count != 0) && (x2OfferLossFrequency != 0) && (LevelLosses.Count % x2OfferLossFrequency == 0)) ||
                                              (result.Win && (Player.Level == 2) && (x2OfferWinFrequency != 0));

                buttonCollectBonus.gameObject.SetActive(shouldShowBonusButton);
                buttonCollect.gameObject.SetActive(shouldShowBonusButton);

                buttonCollectBig.gameObject.SetActive(!shouldShowBonusButton);
                oneButtonLayout.SetActive(!shouldShowBonusButton);

                redSimpleLabel.SetActive(!isSubscriptionActive);

                redBonusLabel.SetActive(isSubscriptionActive);
                purpleBonusLabel.SetActive(isSubscriptionActive);
                purpleBonusLabelBig.SetActive(isSubscriptionActive);
            }

            Show(onHided);

            AudioManager.Instance.Play(showClip, AudioType.Sound);
        }


        public void Hide(bool wasVideoWatched)
        {
            base.Hide();

            Matches.Increment();
            MatchesPerSession.Increment();

            float coinMultiplier = wasVideoWatched ? GetBonusResultMultiplier() : GetSimpleResultMultiplier();

            if (TutorialManager.Instance.IsShootTutorialPassed)
            {
                GameAnalytics.SendMatchFinishEvent(Arsenal.GetWeaponDesc(Player.CurrentWeapon).ToLower(),
                                                   (PlayerConfig.GetResultCoins(arenaResult.Coins) * coinMultiplier).ToString(),
                                                   arenaResult.Win, (Player.Level + 1).ToString(),
                                                   (Arsenal.GetWeaponMaxAmmo(Player.CurrentWeapon) *
                                                   Arsenal.GetWeaponParameters(Player.CurrentWeapon).SizeMagazine).ToString(), arenaResult.MissedShells.ToString(),
                                                   Skins.GetKeyName(Player.CurrentSkin));
            }
            else
            {
                GameAnalytics.FinishFirstTutorialEvent((PlayerConfig.GetResultCoins(arenaResult.Coins) * coinMultiplier).ToString(),
                                                        arenaResult.MissedShells.ToString());
            }

            if ((Player.Level + 1) % 5 != 0 || ((Player.Level + 1) % 5 == 0 && !arenaResult.Win))
            {
                SocialController.TryIncerementMatchesForShowing();
            }

            if (arenaResult.Win)
            {
                Player.UpLevel();
                LevelLosses.Reset();

                if (Player.Level % 5 == 0)
                {
                    RateUs.AllowToShow();
                }
            }
            else
            {
                LevelLosses.Increase();
            }
        }


        #endregion



        #region Private methods

        private void Collect()
        {
            if (!isSubscriptionActive)
            {
                if ((RateUs.CanShowFirstPopUp(Player.Level + 1) || RateUs.CanShowFollowingPopUp(Player.Level + 1)) && !RateUs.WasRated && arenaResult.Win)
                {
                    Player.AddCoins(PlayerConfig.GetResultCoins(arenaResult.Coins) * GetSimpleResultMultiplier());
                    Hide(false);
                    return;
                }

                AdvertisingHelper.ShowInterstitial(AdPlacementType.AfterResult);
            }

            Player.AddCoins(PlayerConfig.GetResultCoins(arenaResult.Coins) * GetSimpleResultMultiplier());
            Hide(false);
        }


        private void CollectBonus()
        {
            float reward = PlayerConfig.GetResultCoins(arenaResult.Coins) * GetBonusResultMultiplier();
            
            AdvertisingHelper.ShowVideo((result) =>
            {
                if (result)
                {
                    Player.AddCoins(reward);
                    Hide(true);
                }
            }, CustomAdPlacementType.LevelResult, reward.ToString());
        }


        private uint GetSimpleResultMultiplier()
        {
            return isSubscriptionActive ? SIMPLE_SUBSCRIPTION_MULTIPLIER : SIMPLE_MULTIPLIER;
        }


        private uint GetBonusResultMultiplier()
        {
            return isSubscriptionActive ? BONUS_SUBSCRIPTION_MULTIPLIER : BONUS_MULTIPLIER;
        }

        #endregion
    }
}