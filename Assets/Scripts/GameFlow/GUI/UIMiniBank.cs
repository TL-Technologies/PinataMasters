using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Modules.Legacy.Tweens;


namespace PinataMasters
{
    public class UIMiniBank : UIUnit<UnitResult>
    {
        [Serializable]
        private class Slot
        {
            public Text coins = null;
            public TextLocalizator freeCoins = null;
            public Text price = null;
            public Button buttonBuy = null;
            public Image videoImage = null;
        }

        #region Variables

        private const string DEFAULT_USD_PRICE_LABEL = "${0}";
        private const string FREE_KEY = "ui.dialog.free";

        public static readonly ResourceGameObject<UIMiniBank> Prefab = new ResourceGameObject<UIMiniBank>("Game/GUI/DialogShop");

        [SerializeField]
        private TweenScale tweenScale = null;
        [SerializeField]
        private TweenImageColor tweenColor = null;
        [SerializeField]
        protected Button closeButton = null;

        [Header("Slots")]
        [SerializeField]
        private Slot[] slots = null;

        private List<Purchases.Config> purchaseToPropose;
        private Purchases.Config[] availablePurchases;
        private IAPs.Name purchaseToBuy;

        private string placement;
        float nextAvailableWeaponPrice = 0;
        bool isLateGameConfig = false;

        bool isVideoShopItemAvailable = false;

        #endregion



        #region Properties

        bool GetVideoShopItemAvailability(float neededCoins, float currentVideoReward)
        {
            if (ABTest.InGameAbTestData.isVideoShopItemEnabled)
            {
                if (ABTest.InGameAbTestData.isCoinsCheckForVideoShopItemEnabled)
                {
                    if (currentVideoReward < neededCoins)
                    {
                        return false;
                    }
                }

                if ((DateTime.Now - GameAnalytics.LastVideoShopItemShowDate).TotalSeconds <
                    ABTest.InGameAbTestData.videoShopItemShowingDelay)
                {
                    return false;
                }

                return true;
            }
            
            return false;
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(Close);
            availablePurchases = Purchases.DefaultPurchases;
        }


        private void Start()
        {
            float scale = GetComponent<UIScaler>().Scale;
            tweenScale.EndScale = new Vector3(scale, scale, tweenScale.EndScale.z);
        }

        #endregion



        #region Public methods

        public void Show(float neededCoins, string showedPlacement, Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);
            
            float videoReward = GetVideoRewardCoins();
            isVideoShopItemAvailable = GetVideoShopItemAvailability(neededCoins, videoReward);

            placement = showedPlacement;
            nextAvailableWeaponPrice = Arsenal.GetNextAvailableWeaponPrice();
            
            purchaseToPropose = (from i in availablePurchases where i.coins + i.freeCoins >= nextAvailableWeaponPrice orderby i.coins ascending select i).
                Take(slots.Length).ToList();

            if (purchaseToPropose.Count == slots.Length)
            {
                var item = availablePurchases.OrderByDescending(i => i.coins).Take(1).ToList();
                purchaseToPropose.RemoveAt(purchaseToPropose.Count - 1);
                purchaseToPropose.AddRange(item);
                isLateGameConfig = false;
            }
            else
            {
                isLateGameConfig = true;
                purchaseToPropose = (from i in Purchases.LateGamePurchases orderby i.coins descending select i).Take(slots.Length).Reverse().ToList();
            }

            for (int i = 0; i < slots.Length; i++)
            {
                InitSlot(i, isVideoShopItemAvailable);
            }

            tweenScale.SetBeginStateImmediately();
            tweenScale.SetEndState(0f,(_) => Showed());

            tweenColor.Duration = tweenScale.duration;
            tweenColor.Play();

            GameAnalytics.BankShowEvent(placement);
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            for (int i = 0; i < slots.Length; i++)
            {
                IAPs.CancelPriceRequest(purchaseToPropose[i].productName, () => SetPrice(i));
                slots[i].buttonBuy.onClick.RemoveAllListeners();
            }
            
            tweenColor.Play(forward: false);
            tweenScale.SetBeginState(0f, (_) => Hided());
        }

        #endregion



        #region Private methods

        private void InitSlot(int i, bool isVideoShopItemAvailable)
        {
            Slot slot = slots[i];
            
            #warning bad architecture for detecting video items 
            if (slot.videoImage != null)
            {
                slot.videoImage.enabled = isVideoShopItemAvailable;
                slot.price.enabled = !isVideoShopItemAvailable;

                if (isVideoShopItemAvailable)
                {
                    InitVideoSlot(slot);
                }
                else
                {
                    InitRealSlot(i);
                }
            }
            else
            {
                InitRealSlot(i);
            }
        }


        private void SetPrice(int i)
        {
            string itemPrice = IAPs.GetPrice(purchaseToPropose[i].productName);
            string price = (string.IsNullOrEmpty(itemPrice)) ? (string.Format(DEFAULT_USD_PRICE_LABEL,
                                                               IAPs.GetUSDPrice(purchaseToPropose[i].productName))) : (itemPrice);

            slots[i].price.text = price;
        }


        private void InitVideoSlot(Slot slot)
        {
            slot.coins.text = GetVideoRewardCoins().ToShortFormat(true);
            slot.freeCoins.gameObject.SetActive(false);
            slot.buttonBuy.onClick.AddListener(() => TryShowVideo());
        }


        private void InitRealSlot(int i)
        {
            float coinsCount = purchaseToPropose[i].coins;
            float freeCoinsCount = purchaseToPropose[i].freeCoins;

            if (isLateGameConfig)
            {
                coinsCount *= nextAvailableWeaponPrice;
                freeCoinsCount *= nextAvailableWeaponPrice;
            }

            slots[i].coins.text = coinsCount.ToShortFormat();

            bool shouldShowFreeCoins = !Mathf.Approximately(freeCoinsCount, 0.0f);
            slots[i].freeCoins.gameObject.SetActive(shouldShowFreeCoins);
            if (shouldShowFreeCoins)
            {
                slots[i].freeCoins.SetParams(freeCoinsCount.ToShortFormat());
            }

            SetPrice(i);

            if (!IAPs.IsPriceActual(purchaseToPropose[i].productName))
            {
                IAPs.RequestPrice(purchaseToPropose[i].productName, () => SetPrice(i));
            }

            slots[i].buttonBuy.onClick.AddListener(() => TryBuy(purchaseToPropose[i].productName));
        }


        private void TryShowVideo()
        {
            float reward = GetVideoRewardCoins();
            
            AdvertisingHelper.ShowVideo((result) =>
            {
                if (result)
                {
                    Player.AddCoins(reward);
                    GameAnalytics.LastVideoShopItemShowDate = DateTime.Now;
                    Hide();
                }
            }, CustomAdPlacementType.MiniBank, reward.ToString());
        }


        private float GetVideoRewardCoins()
        {
            float subscriptionMultiplier = (IAPs.IsSubscriptionActive || IAPs.IsNoSubscriptionActive) ? (2.0f) : (1.0f);
            return ShooterShadowsConfig.GetAllSkinsWeaponsPower() * subscriptionMultiplier * CoinsBooster.asset.Value.CoinsMultiplier *
                Player.GetBonusCoinsSkill() * ABTest.InGameAbTestData.videoShopItemRewardMultiplier;
        }


        private void TryBuy(IAPs.Name productName)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIInfo.Prefab.Instance.Show(UIInfo.Type.NoInternet);
            }
            else
            {
                GameAnalytics.BankTryEvent(placement);

                if (IAPs.IsPriceActual(productName))
                {
                    EventSystemController.DisableEventSystem();
                    UILoader.Prefab.Instance.Show();
                    purchaseToBuy = productName;
                    IAPs.Buy(productName, OnDone);
                }
            }
        }


        private void OnDone(bool isBought)
        {
            UILoader.Prefab.Instance.Hide();
            if (isBought)
            {
                var boughtPurchase = purchaseToPropose.First((i) => i.productName.Equals(purchaseToBuy));
                float multiplier = isLateGameConfig ? nextAvailableWeaponPrice : 1.0f;
                Player.AddCoins((boughtPurchase.coins + boughtPurchase.freeCoins) * multiplier);

                GameAnalytics.BankDoneEvent(placement);
                Hide();
            }

            EventSystemController.EnableEventSystem();
        }


        private void Close()
        {
            GameAnalytics.BankSkipEvent();
            Hide();
        }

        #endregion
    }
}
