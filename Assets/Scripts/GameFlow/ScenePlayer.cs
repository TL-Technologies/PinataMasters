using Modules.InAppPurchase;
using System;
using UnityEngine;


namespace PinataMasters
{
    public class ScenePlayer : MonoBehaviour
    {

        #region Unity lifecycle

        private void Awake()
        {
            UIPlayerMenu.OnSubscriptionClick += OnSubscriptionClick;
            UIPlayerMenu.OnMiniBankClick += ShowMiniBank;
            ViewGun.OnNeedShowMiniBank += ShowMiniBank;
            ViewSkin.OnNeedShowMiniBank += ShowMiniBank;
            OfflinerewardPanel.OnNeedShowMiniBank += ShowMiniBank;
            BonusCoinsPanel.OnNeedShowMiniBank += ShowMiniBank;
            SpeedPanel.OnNeedShowMiniBank += ShowMiniBank;
        }


        private void OnDestroy()
        {
            UIPlayerMenu.OnSubscriptionClick -= OnSubscriptionClick;

            UIPlayerMenu.OnMiniBankClick -= ShowMiniBank;
            ViewGun.OnNeedShowMiniBank -= ShowMiniBank;
            ViewSkin.OnNeedShowMiniBank -= ShowMiniBank;
            OfflinerewardPanel.OnNeedShowMiniBank -= ShowMiniBank;
            BonusCoinsPanel.OnNeedShowMiniBank -= ShowMiniBank;
            SpeedPanel.OnNeedShowMiniBank -= ShowMiniBank;
        }

        #endregion



        #region Public methods

        public void Show(Action OnHidedCallback)
        {
            UIPlayerMenu.Prefab.Instance.Show((_) => OnHidedCallback(), OnMenuShowed);
        }

        #endregion



        #region  Event handlers

        private void OnMenuShowed()
        {
            if (TutorialManager.Instance.IsPrestigeTutorialCanStart)
            {
                return;
            }

            if (RateUs.CanShowFirstPopUp(Player.Level))
            {
                RateUs.ShowNativePopUp();
                return;
            }

            if (RateUs.CanShowFollowingPopUp(Player.Level))
            {
                if (!RateUs.WasRated)
                {
                    RateUs.ShowNativeOrPinataPopUp();
                }

                return;
            }


            LLPromoFetcherUnit unit;

            if (SocialController.CheckForAvailable(out unit))
            {
                UISocialPopUp.Prefab.Instance.Show(unit);
            }
        }


        private void OnSubscriptionClick()
        {
            UISubscriptionSmart.Prefab.Instance.Show(SubscriptionPurchasePlacement.ApplicationMainMenu);
        }


        private void ShowMiniBank(float neededCoins, string placement)
        {
            UIMiniBank.Prefab.Instance.Show(neededCoins, placement);
        }

        #endregion
    }
}
