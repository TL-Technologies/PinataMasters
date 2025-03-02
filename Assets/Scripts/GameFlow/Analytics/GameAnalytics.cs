using Modules.General;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.InAppPurchase;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;


namespace PinataMasters
{
    public static class UserProperties
    {
        public const string TOTAL_PURCHASES_COUNT = "total_purchases_count";

        public const string TOTAL_SPEND_BUCKS = "total_spend_bucks";
        public const string TOTAL_SPEND_COINS = "total_spend_coins";
        public const string TOTAL_EARN_GEMS = "total_earn_gems";

        public const string TOTAL_MINUTES_IN_GAME = "total_minutes_in_game";

        public const string SESSIONS_COUNT = "sessions_count";
        public const string MATCHES_COUNT = "matches_count";

        public const string CHEATERS = "cheaters";
        public const string DIAMOND_MEMBERSHIP_TYPE = "diamond_membership_type";
    }


    public static class MiniBankPlacement
    {
        public const string WEAPON_UPGRADE = "weapon_upgrade";
        public const string CHARACTER_UPGRADE = "character_upgrade";
        public const string BUTTON_WEAPON_GET = "button_weapon_get";

        public const string BUTTON_WEAPON_BUY = "button_weapon_buy";
        public const string BUTTON_MENU_BANK = "button_menu_bank";
        public const string BUTTON_SKINS_GET = "button_skins_get";
    }


    public static class ContentClass
    {
        public const string SKIN = "skin";
        public const string WEAPON = "weapon";
        public const string CHARACTER = "character";
    }


    public static class ContentType
    {
        public const string AMMO = "ammo";
        public const string DAMAGE = "damage";
        public const string SPEED = "speed";
        public const string OFFLINE_REWARD = "offline_reward";
        public const string BONUS_COINS = "bonus_coins";
    }


    public static class UnlockType
    {
        public const string INAPP = "inapp";
        public const string INGAME_PURCHASE = "ingame_purchase";
    }


    public static class CurrencyType
    {
        public const string COINS = "coins";
        public const string GEMS = "gems";
    }


    public static class SubscriptionType
    {
        public const string START = "start";
        public const string BUTTON = "button";
        public const string NO_SUBSCRIPTION = "no_subscription";
    }


    public static class GameAnalytics
    {
        #region Variables

        private const int EVENTS_UPGRADE_STEP = 5;
        private const string TOTAL_INGAME_MINUTES = "total_ingame_minutes";
        private const string PURCHASES_WITHOUT_SUBSCRIPTION = "purchases_without_subscription";
        private const string PURCHASES_WITHOUT_WEEK_SUBSCRIPTION = "purchases_without_week_subscription";

        private const string SPENT_COINS = "spent_coins";
        private const string SPENT_BUCKS = "spent_bucks";
        private const string EARNED_GEMS = "earned_gems";

        const string LAST_VIDEO_SHOP_ITEM_SHOW_DATE = "last_video_shop_item_show_date";

        #endregion



        #region Properties

        public static bool ShouldRepeatUpgradeTutorial { get; set; } = false;
        public static bool ShouldRepeatUpgradeCharacterTutorial { get; set; } = false;
        public static int EventUpgradeStep { get { return EVENTS_UPGRADE_STEP; } }
        
        
        public static DateTime LastVideoShopItemShowDate
        {
            get { return CustomPlayerPrefs.GetDateTime(LAST_VIDEO_SHOP_ITEM_SHOW_DATE, DateTime.MinValue); }
            set { CustomPlayerPrefs.SetDateTime(LAST_VIDEO_SHOP_ITEM_SHOW_DATE, value); }
        }

        
        private static float SpentBucks
        {
            get
            {
                return CustomPlayerPrefs.GetFloat(SPENT_BUCKS);
            }
            set
            {
                CustomPlayerPrefs.SetFloat(SPENT_BUCKS, value);
                SetUserProperty(UserProperties.TOTAL_SPEND_BUCKS, SpentBucks.ToString());
            }
        }


        private static float SpentCoins
        {
            get
            {
                return CustomPlayerPrefs.GetFloat(SPENT_COINS);
            }
            set
            {
                CustomPlayerPrefs.SetFloat(SPENT_COINS, value);
                SetUserProperty(UserProperties.TOTAL_SPEND_COINS, SpentCoins.ToString());
            }
        }


        private static float EarnedGems
        {
            get
            {
                return CustomPlayerPrefs.GetFloat(EARNED_GEMS);
            }
            set
            {
                CustomPlayerPrefs.SetFloat(EARNED_GEMS, value);
                SetUserProperty(UserProperties.TOTAL_EARN_GEMS, EarnedGems.ToString());
            }
        }


        private static double TotalGameMinutes
        {
            get
            {
                return Math.Round(SavedGameMinutes + (TimeCollector.Value / 60f), 2);
            }
        }


        private static double SavedGameMinutes
        {
            get
            {
                return CustomPlayerPrefs.GetDouble(TOTAL_INGAME_MINUTES, 0);
            }
            set
            {
                CustomPlayerPrefs.SetDouble(TOTAL_INGAME_MINUTES, value);
            }
        }


        private static int PurchasesWithoutSubscription
        {
            get
            {
                return CustomPlayerPrefs.GetInt(PURCHASES_WITHOUT_SUBSCRIPTION, 0);
            }
            set
            {
                CustomPlayerPrefs.SetInt(PURCHASES_WITHOUT_SUBSCRIPTION, value);
            }
        }


        private static int PurchasesWithoutWeekSubscription
        {
            get
            {
                return CustomPlayerPrefs.GetInt(PURCHASES_WITHOUT_WEEK_SUBSCRIPTION, 0);
            }
            set
            {
                CustomPlayerPrefs.SetInt(PURCHASES_WITHOUT_WEEK_SUBSCRIPTION, value);
            }
        }

        #endregion



        #region Public methods

        public static void Initialize()
        {
            LLApplicationStateRegister.OnApplicationEnteredBackground += OnApplicationEnteredBackground;
            Sheduler.OnDestred += GameQuit;

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Analytics.CustomEvent("session_start_internet");
                SendFirebaseAnalytics("session_start_internet");
            }
            else
            {
                Analytics.CustomEvent("session_start_nointernet");
                SendFirebaseAnalytics("session_start_nointernet");
            }
        }


        public static void ShowSubcriptionEvent(string subscriptionType)
        {
            Analytics.CustomEvent("subcription_show", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });

            SendFirebaseAnalytics("subcription_show", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });
        }


        public static void TrySubcriptionEvent(string subscriptionType)
        {
            Analytics.CustomEvent("subcription_try", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });

            SendFirebaseAnalytics("subcription_try", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });
        }


        public static void DoneSubcriptionEvent(string subscriptionType)
        {
            Analytics.CustomEvent("subcription_done", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });

            SendFirebaseAnalytics("subcription_done", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });
        }


        public static void SkipSubcriptionEvent(string subscriptionType)
        {
            Analytics.CustomEvent("subcription_skip", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });

            SendFirebaseAnalytics("subcription_skip", new Dictionary<string, object>()
            {
                { "subsсription_type", subscriptionType }
            });
        }


        public static void SendTryRestoreEvent()
        {
            Analytics.CustomEvent("subscription_start_restore_try");
            SendFirebaseAnalytics("subscription_start_restore_try");
        }


        public static void SendDoneRestoreEvent()
        {
            Analytics.CustomEvent("subscription_start_restore_done");
            SendFirebaseAnalytics("subscription_start_restore_done");
        }


        public static void SendUnlockContentEvent(string contentClass, string contentType, string unlockType)
        {
            Analytics.CustomEvent("unlock_content", new Dictionary<string, object>()
            {
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "unlock_type", unlockType }
            });

            SendFirebaseAnalytics("unlock_content", new Dictionary<string, object>()
            {
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "unlock_type", unlockType }
            });
        }


        public static void TrySendUpgradeContentEvent(string contentClass, string contentType, uint upgradeValue)
        {
            if (upgradeValue % EVENTS_UPGRADE_STEP == 0)
            {
                Analytics.CustomEvent("upgrade_content", new Dictionary<string, object>()
            {
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "upgrade_value", upgradeValue.ToString() }
            });

                SendFirebaseAnalytics("upgrade_content", new Dictionary<string, object>()
            {
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "upgrade_value", upgradeValue.ToString() }
            });
            }
        }


        public static void SendSpendIngameCurrencyEvent(string currencyAmount, string contentClass, string contentType, string type, uint upgradeValue = 0)
        {
            if (upgradeValue % EVENTS_UPGRADE_STEP == 0)
            {
                Analytics.CustomEvent("spend_ingame_currency", new Dictionary<string, object>()
            {
                { "currency_type", type },
                { "currency_amount", currencyAmount },
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

                SendFirebaseAnalytics("spend_ingame_currency", new Dictionary<string, object>()
            {
                { "currency_type", type },
                { "currency_amount", currencyAmount },
                { "content_class", contentClass },
                { "content_type", contentType },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
            }
        }


        public static void SendGiftClaimEvent(int day)
        {
            Analytics.CustomEvent("gift_claim", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "gift_id", "day_" + day.ToString() }
            });

            SendFirebaseAnalytics("gift_claim", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "gift_id", "day_" + day.ToString() }
            });
        }


        public static void SendClaimRewardEvent()
        {
            Analytics.CustomEvent("off_claim", TotalMinutesDictionary());
            SendFirebaseAnalytics("off_claim", TotalMinutesDictionary());
        }


        public static void SendMatchStartEvent(string contentType, string level, string totalBullets)
        {
            Analytics.CustomEvent("match_start", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "content_type", contentType },
                { "lvl", level },
                { "bullet_total", totalBullets }
            });

            SendFirebaseAnalytics("match_start", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "content_type", contentType },
                { "lvl", level },
                { "bullet_total", totalBullets }
            });
        }


        public static void SendMatchFinishEvent(string contentType, string coinsTotal, bool isWin, string level, string bulletTotal, string missedBullets, string skinName)
        {
            Analytics.CustomEvent("match_finish", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "content_type", contentType },
                { "coins_total", coinsTotal },
                { "result_type", isWin ? "win" : "lose" },
                { "lvl", level },
                { "bullet_total", bulletTotal },
                { "misses_total", missedBullets }
            });

            SendFirebaseAnalytics("match_finish", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "content_type", contentType },
                { "coins_total", coinsTotal },
                { "result_type", isWin ? "win" : "lose" },
                { "lvl", level },
                { "bullet_total", bulletTotal },
                { "misses_total", missedBullets },
                { "skin_name", skinName }
            });
        }


        public static void StartFirstTutorialEvent()
        {
            Analytics.CustomEvent("tutorial_1_start", TotalMinutesDictionary());
            SendFirebaseAnalytics("tutorial_1_start", TotalMinutesDictionary());
        }


        public static void FinishFirstTutorialEvent(string totalCoins, string missedBullets)
        {
            Analytics.CustomEvent("tutorial_1_finish", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "coins_total", totalCoins }
            });

            SendFirebaseAnalytics("tutorial_1_finish", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "coins_total", totalCoins },
            });
        }


        public static void SendGDPRSettingClickEvent()
        {
            Analytics.CustomEvent("settings_popup_terms_of_use_click");
            SendFirebaseAnalytics("settings_popup_terms_of_use_click");
        }


        public static void TrySendPurchaseCountUnityAnalytics(string postfixProductID)
        {
            PurchasesWithoutSubscription++;
            switch (PurchasesWithoutSubscription)
            {
                case 1:
                    Analytics.CustomEvent("purchase_first", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                case 2:
                    Analytics.CustomEvent("purchase_second", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                case 3:
                    Analytics.CustomEvent("purchase_third", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                default:
                    break;
            }
        }


        public static void TrySendPurchaseCountFirebase(string postfixProductID)
        {
            PurchasesWithoutWeekSubscription++;
            switch (PurchasesWithoutWeekSubscription)
            {
                case 1:
                    SendFirebaseAnalytics("purchase_first", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                case 2:
                    SendFirebaseAnalytics("purchase_second", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                case 3:
                    SendFirebaseAnalytics("purchase_third", new Dictionary<string, object>()
                    {
                        { "product_id", postfixProductID },
                        { "minutes_in_game", TotalGameMinutes.ToString() }
                    });
                    break;

                default:
                    break;
            }

            SetUserProperty(UserProperties.TOTAL_PURCHASES_COUNT, PurchasesWithoutWeekSubscription.ToString());
        }


        public static void SendIAPPurchaseUnityAnalytics(IPurchaseItemResult result)
        {
            Analytics.CustomEvent("inapp_purchase", new Dictionary<string, object>()
            {
                { "validation_type", result.ValidationState.ToString().ToLower() },
                { "product_id", IAPs.GetPostfixProductID(result.StoreItem.ProductId) },
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "matches_count", Matches.Count.ToString() }

            });
        }


        public static void BankTryEvent(string placement)
        {
            Analytics.CustomEvent("bank_try", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });

            SendFirebaseAnalytics("bank_try", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });
        }


        public static void BankDoneEvent(string placement)
        {
            Analytics.CustomEvent("bank_done", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });

            SendFirebaseAnalytics("bank_done", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });
        }


        public static void BankShowEvent(string placement)
        {
            Analytics.CustomEvent("bank_show", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });

            SendFirebaseAnalytics("bank_show", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "placement", placement }

            });
        }


        public static void BankSkipEvent()
        {
            Analytics.CustomEvent("bank_skip", TotalMinutesDictionary());
            SendFirebaseAnalytics("bank_skip", TotalMinutesDictionary());
        }


        public static void IngameBonusShow(string placement)
        {
            Analytics.CustomEvent("bonus_show", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

            SendFirebaseAnalytics("bonus_show", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
        }


        public static void IngameBonusClick(string placement)
        {
            Analytics.CustomEvent("bonus_click", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

            SendFirebaseAnalytics("bonus_click", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
        }


        public static void IngameOfferSkip(string placement)
        {
            Analytics.CustomEvent("bonus_deny", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

            SendFirebaseAnalytics("bonus_deny", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
        }


        public static void IngameOfferTry(string placement)
        {
            Analytics.CustomEvent("bonus_try", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

            SendFirebaseAnalytics("bonus_try", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
        }


        public static void IngameOfferDone(string placement)
        {
            Analytics.CustomEvent("bonus_done", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });

            SendFirebaseAnalytics("bonus_done", new Dictionary<string, object>()
            {
                { "placement", placement },
                { "minutes_in_game", TotalGameMinutes.ToString() }
            });
        }


        public static void SetCoinsSpentUserProperty(float coins)
        {
            SpentCoins += coins;
        }


        public static void SetGemsEarnedUserProperty(float gems)
        {
            EarnedGems += gems;
        }


        public static void SetBucksSpentUserProperty(float bucks)
        {
            SpentBucks += bucks;
        }


        public static void SetSessionsUserProperty()
        {
            SetUserProperty(UserProperties.SESSIONS_COUNT, Sessions.Count.ToString());
        }


        public static void SetMatchesUserProperty()
        {
            SetUserProperty(UserProperties.MATCHES_COUNT, Matches.Count.ToString());
        }


        public static void SetCheaterUserProperty()
        {
            SetUserProperty(UserProperties.CHEATERS, "true");
        }


        public static void SendSubscriptionActivityAnalytics()
        {
            ISubscriptionInfo result = Services.GetService<IStoreManager>().GetSubscriptionsForDate(SubscriptionTimer.DateToCheck).FirstOrDefault();

            if (result != null)
            {
                string propertyValue = "simple";
                if (IAPs.GetProductID(IAPs.Name.SubscriptionWeekly) == result.ProductId)
                {
                    propertyValue = "weekly";
                }
                else if (IAPs.GetProductID(IAPs.Name.SubscriptionMonthly) == result.ProductId)
                {
                    propertyValue = "monthly";
                }
                else if (IAPs.GetProductID(IAPs.Name.SubscriptionYearly) == result.ProductId)
                {
                    propertyValue = "yearly";
                }

                SetUserProperty(UserProperties.DIAMOND_MEMBERSHIP_TYPE, propertyValue);
            }
        }



        public static void ResetLevelClick()
        {
            SendFirebaseAnalytics("reset_level_click", TotalMinutesDictionary());
        }


        public static void ResetLevelSkip()
        {
            SendFirebaseAnalytics("reset_level_skip", TotalMinutesDictionary());
        }


        public static void ResetLevelDone(uint level, float gems)
        {
            SendFirebaseAnalytics("reset_level_done", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "lvl", level.ToString() },
                { "gems_earned", gems.ToString() }
            });
        }


        public static void BuySkin(string skinName, float price)
        {
            SendFirebaseAnalytics("skin_buy_done", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "skin_name", skinName },
                { "price", price.ToString() }
            });
        }


        public static void UpgradeSkin(string skinName, float price, uint upgradeLevel)
        {
            SendFirebaseAnalytics("skin_upgrade", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "skin_name", skinName },
                { "price", price.ToString() },
                { "lvl_upgrade", upgradeLevel.ToString() }
            });
        }


        public static void ChooseSkin(string skinName, uint upgradeLevel)
        {
            SendFirebaseAnalytics("skin_choose", new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() },
                { "skin_name", skinName },
                { "lvl_upgrade", upgradeLevel.ToString() }
            });
        }
        
        #endregion



        #region Private methods

        private static void OnApplicationEnteredBackground(bool isEnteredBackground)
        {
            if (isEnteredBackground)
            {
                SetUserProperty(UserProperties.TOTAL_MINUTES_IN_GAME, TotalGameMinutes.ToString());
                SavedGameMinutes += (TimeCollector.Value / 60f);
                TimeCollector.Reset();
            }
        }


        private static void GameQuit()
        {
            SavedGameMinutes = TotalGameMinutes;
        }


        private static Dictionary<string, object> TotalMinutesDictionary()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>()
            {
                { "minutes_in_game", TotalGameMinutes.ToString() }
            };

            return dictionary;
        }


        private static void SetUserProperty(string propertyName, string propertyValue)
        {
//            Services.GetService<FirebaseAnalyticsServiceImplementor>()?.SetUserProperty(propertyName, propertyValue);
        }
        
        
        private static void SendFirebaseAnalytics(string eventName, Dictionary<string,object> dictionary = null)
        {
//                if (!Debug.isDebugBuild && !string.IsNullOrEmpty(eventName))
//                {
//                    if (dictionary == null)
//                    {
//                        Services.AnalyticsManager.SendEvent(typeof(FirebaseAnalyticsServiceImplementor), eventName, null);
//                    }
//                    else
//                    {
//                        Dictionary<string, string> stringValues = dictionary.ToDictionary(value => value.Key, value => value.Value.ToString());
//
//                        Services.AnalyticsManager.SendEvent(typeof(FirebaseAnalyticsServiceImplementor), eventName, stringValues);
//                    }
//                }
        }

        #endregion
    }
}
