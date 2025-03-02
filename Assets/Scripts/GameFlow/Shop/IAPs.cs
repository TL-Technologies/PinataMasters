using Modules.AppsFlyer;
using Modules.General;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.InAppPurchase;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    public static class IAPs
    {
        public enum Name
        {
            SubscriptionWeekly,
            SubscriptionMonthly,
            SubscriptionYearly,
            NoSubscription,
            PackS,
            PackM,
            PackL,
            PackXS,
            PackXM,
            PackXL,
            PackXXS,
            PackXXM,
            PackXXL,
            PackXXXS,
            PackXXXM,
            PackXXXL
        }

        public enum Type
        {
            Subscription,
            Consumable,
            Nonconsumable,
        }

        #region Variables

        private const string NOSUBSCRIPTION_KEY = "noSubscription";
        private const string FAKESUBSCRIPTION_KEY = "fakeSubscription";

        private class IAPInfo
        {
            public Type type;
            public string productID;
            public int priceTier;
            public bool isPriceCorrect;
        }

        private readonly static Dictionary<Name, IAPInfo> IAPsData = new Dictionary<Name, IAPInfo>
        {
            { Name.SubscriptionWeekly, new IAPInfo { type = Type.Subscription, productID = "dimondweekly", priceTier = 8} },
            { Name.SubscriptionMonthly, new IAPInfo { type = Type.Subscription, productID = "dimondmonthly", priceTier = 20} },
            { Name.SubscriptionYearly, new IAPInfo { type = Type.Subscription, productID = "dimondeyearly", priceTier = 60} },

            { Name.NoSubscription, new IAPInfo { type = Type.Nonconsumable, productID = "nosubscription", priceTier = 30} },

            { Name.PackS, new IAPInfo { type = Type.Consumable, productID = "pack.s", priceTier = 2} },
            { Name.PackM, new IAPInfo { type = Type.Consumable, productID = "pack.m", priceTier = 3} },
            { Name.PackL, new IAPInfo { type = Type.Consumable, productID = "pack.l", priceTier = 5} },
            { Name.PackXS, new IAPInfo { type = Type.Consumable, productID = "pack.xs", priceTier = 10} },
            { Name.PackXM, new IAPInfo { type = Type.Consumable, productID = "pack.xm", priceTier = 15} },
            { Name.PackXL, new IAPInfo { type = Type.Consumable, productID = "pack.xl", priceTier = 20} },
            { Name.PackXXS, new IAPInfo { type = Type.Consumable, productID = "pack.xxs", priceTier = 25} },
            { Name.PackXXM, new IAPInfo { type = Type.Consumable, productID = "pack.xxm", priceTier = 30} },
            { Name.PackXXL, new IAPInfo { type = Type.Consumable, productID = "pack.xxl", priceTier = 35} },
            { Name.PackXXXS, new IAPInfo { type = Type.Consumable, productID = "pack.xxxs", priceTier = 40} },
            { Name.PackXXXM, new IAPInfo { type = Type.Consumable, productID = "pack.xxxm", priceTier = 50} },
            { Name.PackXXXL, new IAPInfo { type = Type.Consumable, productID = "pack.xxxl", priceTier = 60} }
        };

        private static readonly Dictionary<Name, Action> callbacks = new Dictionary<Name, Action>();

        private static Action<bool> onIAPBought = delegate { };
        private static Action<bool> restoreCallback = delegate { };
        private static IStoreManager storeManager = null;
        #if !UNITY_EDITOR
            private static bool isAllProductsReceived;
        #endif
        #endregion



        #region Properties

        public static bool IsAnySubscriptionActive => IsSubscriptionActive || IsNoSubscriptionActive;


        public static bool IsTrialSubscriptionAvailable
        {
            get
            {
                return StoreManager.IsSubscriptionTrialAvailable(IAPsData[Name.SubscriptionWeekly].productID);
            }
        }


        public static bool IsSubscriptionActive
        {
            get
            {
                ISubscriptionInfo currentSubscriptionReward = StoreManager.GetSubscriptionsForDate(SubscriptionTimer.DateToCheck).FirstOrDefault();
                bool active = currentSubscriptionReward == null ? false : true;

                bool isSubscriptionActive = active || SubscriptionTimer.IsLastSubscriptionActive;
                SubscriptionTimer.IsLastSubscriptionActive = active;
                #if UNITY_EDITOR
                return IsFakeSubscriptionActive;
                #else
                return isSubscriptionActive;
                #endif
            }
        }


        public static bool IsNoSubscriptionActive
        {
            get
            {
                return CustomPlayerPrefs.GetBool(NOSUBSCRIPTION_KEY, false) || StoreManager.IsStoreItemPurchased(IAPsData[Name.NoSubscription].productID);
            }

            set
            {
                CustomPlayerPrefs.SetBool(NOSUBSCRIPTION_KEY, value, true);
            }
        }


        public static bool IsFakeSubscriptionActive
        {
            get
            {
                return CustomPlayerPrefs.GetBool(FAKESUBSCRIPTION_KEY, false);
            }

            set
            {
                CustomPlayerPrefs.SetBool(FAKESUBSCRIPTION_KEY, value, true);
            }
        }


        public static IStoreManager StoreManager
        {
            get
            {
                if (storeManager == null)
                {
                    storeManager = Services.GetService<IStoreManager>();
                }

                return storeManager;
            }
        }

        #endregion



        #region Unity lifecycle

        public static void Initialize()
        {
            foreach (Name t in Enum.GetValues(typeof(Name)))
            {
                callbacks[t] = delegate { };
            }

            ItemsInitialization();

            StoreManager.PurchaseComplete += StoreManager_PurchaseComplete;
            StoreManager.ItemDataReceived += StoreManager_ItemDataReceived;
            StoreManager.RestorePurchasesComplete += StoreManager_RestorePurchasesComplete;

            #if UNITY_EDITOR
            foreach (IAPInfo info in IAPsData.Values)
            {
                info.isPriceCorrect = true;
            }
            #endif
        }

        #endregion



        #region Public methods

        public static float GetUSDPrice(Name type)
        {
            return ConvertTierToUSD(IAPsData[type].priceTier);
        }


        public static string GetPostfixProductID(string productID)
        {
            return productID;
        }

        public static string GetProductID(Name type)
        {
            return IAPsData[type].productID;
        }


        public static string GetPrice(Name type)
        {
            string result = string.Empty;
            #if UNITY_EDITOR
                result = string.Concat(ConvertTierToUSD(IAPsData[type].priceTier), "$");
            #else
                IAPInfo purchase = IAPsData[type];

                if (purchase.isPriceCorrect)
                {
                    IStoreItem storeItem = StoreManager.GetStoreItem(purchase.productID);
                    result = storeItem.LocalizedPrice;
                }
            #endif
            return result;
        }


        public static void RequestPrice(Name type, Action onDone)
        {
            callbacks[type] += onDone;
        }


        public static void CancelPriceRequest(Name type, Action onDone)
        {
            callbacks[type] -= onDone;
        }


        public static void Buy(Name type, Action<bool> onDone)
        {
            onIAPBought = onDone;

            IAPInfo item = IAPsData[type];

            if (item.isPriceCorrect)
            {

                #if UNITY_EDITOR
                    Sheduler.PlayCoroutine(FakePurchase(item));
                #else
                    StoreManager.PurchaseItem(item.productID, null);
                #endif
            }
        }


        public static bool IsPriceActual(Name type)
        {
            return IAPsData[type].isPriceCorrect;
        }


        public static void RestorePurchase(Action<bool> callback)
        {
            restoreCallback = callback;
            StoreManager.RestorePurchases();

            #if UNITY_EDITOR 
                Sheduler.PlayCoroutine(FakeRestore());
            #endif
        }


        public static string GetPriceCurrencyCode(string productID)
        {
            string result = StoreManager.GetStoreItem(productID).CurrencyCode;

            return result ?? string.Empty;
        }


        public static string GetRealPrice(string productID)
        {
            string result = StoreManager.GetStoreItem(productID).RealPrice;
             
            return result ?? string.Empty;
        }

        #endregion



        #region Private methods


        #if UNITY_EDITOR
            private static System.Collections.IEnumerator FakeRestore()
            {
                yield return new WaitForSeconds(3f);
                restoreCallback(true);
            }


            private static System.Collections.IEnumerator FakePurchase(IAPInfo item)
            {
                yield return new WaitForSeconds(3f);

                var iap = IAPsData.First((i) => i.Value.productID.Equals(item.productID));
                if (iap.Key == Name.NoSubscription)
                {
                    IsNoSubscriptionActive = true;
                }

                onIAPBought(true);
                onIAPBought = delegate { };
            }
        #endif


        private static void ItemsInitialization()
        {
            IStoreItem storeItem = null;
            foreach (KeyValuePair<Name, IAPInfo> info in IAPsData)
            {
                storeItem = StoreManager.GetStoreItem(info.Value.productID);
                if (storeItem != null && info.Value.productID.Equals(storeItem.ProductId) && storeItem.Status == StoreItemStatus.Actual)
                {
                    info.Value.isPriceCorrect = true;
                    callbacks[info.Key]();
                    callbacks[info.Key] = delegate { };
                }
            }

            TrySendActiveSubscriptionEvent();
        }


        private static void StoreManager_ItemDataReceived(IStoreItem storeItem)
        {
            foreach (KeyValuePair<Name, IAPInfo> info in IAPsData)
            {
                if (info.Value.productID.Equals(storeItem.ProductId))
                {
                    info.Value.isPriceCorrect = true;
                    callbacks[info.Key]();
                    callbacks[info.Key] = delegate {};
                }
            }

            TrySendActiveSubscriptionEvent();
        }


        private static void TrySendActiveSubscriptionEvent()
        {
            #if !UNITY_EDITOR
                if (!isAllProductsReceived)
                {
                    isAllProductsReceived = true;

                    if (StoreManager.HasAnyActiveSubscription)
                    {
                        GameAnalytics.SendSubscriptionActivityAnalytics();
                    }
                }
            #endif
        }


        private static bool StoreManager_PurchaseComplete(IPurchaseItemResult result)
        {
            if (result.IsSucceeded)
            {
                var iap = IAPsData.First((i) => i.Value.productID.Equals(result.StoreItem.ProductId));

                if (iap.Value.productID != null)
                {
                    if (iap.Value.type != Type.Subscription)
                    {
                        GameAnalytics.SendIAPPurchaseUnityAnalytics(result);

                        GameAnalytics.TrySendPurchaseCountUnityAnalytics(GetPostfixProductID(result.StoreItem.ProductId));
                    }
                    else
                    {
                        GameAnalytics.SendSubscriptionActivityAnalytics();
                    }

                    if (iap.Key != Name.SubscriptionWeekly)
                    {
                        GameAnalytics.TrySendPurchaseCountFirebase(GetPostfixProductID(result.StoreItem.ProductId));
                    }

                    if (iap.Key == Name.NoSubscription)
                    {
                        IsNoSubscriptionActive = true;
                    }

                    GameAnalytics.SetBucksSpentUserProperty(ConvertTierToUSD(iap.Value.priceTier));

                    onIAPBought(true);
                    onIAPBought = delegate { };
                }

                return true;
            }
            else
            {
                var iap = IAPsData.Values.First((i) => i.productID.Equals(result.StoreItem.ProductId));

                if (iap != null)
                {
                    onIAPBought(false);
                    onIAPBought = delegate { };
                }

                return false;
            }
        }
         

        private static void StoreManager_RestorePurchasesComplete(IRestorePurchasesResult result)
        {
            restoreCallback(result.IsSucceeded);
            restoreCallback = delegate { };
        }


        private static float ConvertTierToUSD(int priceTier)
        {
            float result = 0.01f;

            if (priceTier <= 0)
            {
                result = 0.01f;
            }
            else if (priceTier <= 50)
            {
                result = (float)priceTier;
            }
            else
            {
                switch (priceTier)
                {
                    case 51: result = 55f; break;
                    case 52: result = 60f; break;
                    case 53: result = 65f; break;
                    case 54: result = 70f; break;
                    case 55: result = 75f; break;
                    case 56: result = 80f; break;
                    case 57: result = 85f; break;
                    case 58: result = 90f; break;
                    case 59: result = 95f; break;

                    case 60: result = 100f; break;
                    case 61: result = 110f; break;
                    case 62: result = 120f; break;
                    case 63: result = 125f; break;
                    case 64: result = 130f; break;
                    case 65: result = 140f; break;
                    case 66: result = 150f; break;
                    case 67: result = 160f; break;
                    case 68: result = 170f; break;
                    case 69: result = 175f; break;

                    case 70: result = 180f; break;
                    case 71: result = 190f; break;
                    case 72: result = 200f; break;
                    case 73: result = 210f; break;
                    case 74: result = 220f; break;
                    case 75: result = 230f; break;
                    case 77: result = 240f; break;
                    case 78: result = 250f; break;
                    case 79: result = 300f; break;
                    case 80: result = 400f; break;

                    case 81: result = 450f; break;
                    case 82: result = 500f; break;
                    case 83: result = 600f; break;
                    case 84: result = 700f; break;
                    case 85: result = 800f; break;
                    case 86: result = 900f; break;
                    case 87: result = 1000f; break;
                }
            }

            return result - 0.01f;
        }

        #endregion
    }
}
