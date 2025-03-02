using Modules.General.Abstraction;
using Modules.Notification.Obsolete;
using System;
using UnityEngine;
using UnityEngine.UI;



namespace PinataMasters
{
    public class UIDebugMenu : UITweensUnit<UnitResult>
    {
        #region Variables

        private const string OFFLINE_REWARD_TEXT_KEY = "localnotification.offlinereward";
        private const string DAILY_GIFT_TEXT_KEY = "localnotification.dailygift";

        public static readonly ResourceGameObject<UIDebugMenu> Prefab = new ResourceGameObject<UIDebugMenu>("Game/GUI/DialogDebug");

        [Header("Buttons")]
        [SerializeField]
        private Button interstitial = null;
        [SerializeField]
        private Button reward = null;
        [SerializeField]
        private Button noInternet = null;
        [SerializeField]
        private Button wentWrong = null;
        [SerializeField]
        private Button noAds = null;

        [SerializeField]
        private Button miniBank = null;

        [SerializeField]
        private Button settings = null;
        [SerializeField]
        private Button rateUs = null;
        [SerializeField]
        private Button sociaLike = null;
        [SerializeField]
        private Button sociaSubscribe = null;
        [SerializeField]
        private Button offlineReward = null;

        [SerializeField]
        private Button dailyGift = null;
        [SerializeField]
        private Button smartSubscription = null;
        [SerializeField]
        private Button startSubscription = null;
        [SerializeField]
        private Button buttonNotifications = null;
        [SerializeField]
        private Button buttonRestoreSucces = null;

        [SerializeField]
        private Button buttonClose = null;

        #endregion


        #if DEBUG_TARGET
            #region Unity lifecycle

            protected override void Awake()
            {
                base.Awake();

                interstitial.onClick.AddListener(() => AdvertisingHelper.ShowInterstitial(AdPlacementType.DefaultPlacement));
                reward.onClick.AddListener(() => AdvertisingHelper.ShowVideo(null, AdPlacementType.DefaultPlacement));

                noInternet.onClick.AddListener(ShowNoInternet);
                wentWrong.onClick.AddListener(ShowWentWrong);
                noAds.onClick.AddListener(NoAds);

                miniBank.onClick.AddListener(MiniBank);

                settings.onClick.AddListener(DialogSettings);
                rateUs.onClick.AddListener(Rateus);
                sociaLike.onClick.AddListener(SocialLike);
                sociaSubscribe.onClick.AddListener(SocialSubscribe);
                offlineReward.onClick.AddListener(OfflineReward);

                dailyGift.onClick.AddListener(DailyGift);
                smartSubscription.onClick.AddListener(SmartSubscription);
                startSubscription.onClick.AddListener(StartSubscription);
                buttonNotifications.onClick.AddListener(RegisteLocalNotification);
                buttonRestoreSucces.onClick.AddListener(RestoreSucces);

                buttonClose.onClick.AddListener(() => Hide());
            }

            #endregion



            #region Private methods

            private void ShowNoInternet()
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
                Hide();
            }


            private void ShowWentWrong()
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.SomethingWrong);
                Hide();
            }


            private void NoAds()
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoAds);
                Hide();
            }


            private void MiniBank()
            {
                UIMiniBank.Prefab.Instance.Show(float.MaxValue, "DebugMenu");
                Hide();
            }


            private void Rateus()
            {
                UIRateUs.Prefab.Instance.Show();
                Hide();
            }


            private void DialogSettings()
            {
                UISettings.Prefab.Instance.Show();
                Hide();
            }

            private void SocialLike()
            {
                LLPromoFetcherUnit LLPromoFetcherUnit = new LLPromoFetcherUnit();
                LLPromoFetcherUnit.promoType = LLPromoType.Like;
                UISocialPopUp.Prefab.Instance.Show(LLPromoFetcherUnit);
                Hide();
            }


            private void SocialSubscribe()
            {
                LLPromoFetcherUnit LLPromoFetcherUnit = new LLPromoFetcherUnit();
                LLPromoFetcherUnit.promoType = LLPromoType.Subscription;
                UISocialPopUp.Prefab.Instance.Show(LLPromoFetcherUnit);
                Hide();
            }


            private void OfflineReward()
            {
                UIOfflineReward.Prefab.Instance.Show(null);
                Hide();
            }


            private void SmartSubscription()
            {
                UISubscriptionSmart.Prefab.Instance.Show(null);
                Hide();
            }


            private void DailyGift()
            {
                UIDailyGift.Prefab.Instance.Show(null);
                Hide();
            }


            private void StartSubscription()
            {
                UISubscriptionStart.Prefab.Instance.Show();
                Hide();
            }


            private void RestoreSucces()
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.Restore);
                Hide();
            }


            private static void RegisteLocalNotification()
            {
                LLNotificationManager.ScheduleLocalNotification("NOTIFICATIONS", 
                    LocalisationManager.LocalizedStringOrSource(OFFLINE_REWARD_TEXT_KEY),
                 Application.productName, LocalisationManager.LocalizedStringOrSource(OFFLINE_REWARD_TEXT_KEY), 
                    DateTime.Now + new TimeSpan(0, 1, 0));

                LLNotificationManager.ScheduleLocalNotification("NOTIFICATIONS", 
                    LocalisationManager.LocalizedStringOrSource(DAILY_GIFT_TEXT_KEY), Application.productName, 
                 LocalisationManager.LocalizedStringOrSource(DAILY_GIFT_TEXT_KEY),
                    DateTime.Now + new TimeSpan(0, 1, 0));
            }

            #endregion
        #endif
    }
}

