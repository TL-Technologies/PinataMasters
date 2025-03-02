using Modules.General;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.General.Utilities;
using System;
using System.Linq;


namespace Modules.InAppPurchase
{
    public class SubscriptionRewardManager : ISubscriptionRewardManager
    {
        #region Fields

        private const string SubscriptionRewardDateKey = "subscription_reward_date";

        private IStoreManager StoreManager;

        #endregion



        #region Properties

        public bool IsRewardPopupAvailable
        {
            get
            {
                return SubscriptionDaysForReward > 0 && StoreManager.HasAnyActiveSubscription;
            }
        }
        

        public int Order => 10;


        public int ServiceId => Order;


        public int SubscriptionDaysForReward
        {
            get
            {
                DateTime lastReward = LastSubscriptionRewardDate;
                DateTime checkedDate = StoreUtilities.VerifiedDateTime;
                TimeSpan span = new TimeSpan();

                if (lastReward != DateTime.MinValue)
                {
                    ISubscriptionInfo lastSubscriptionReward = StoreManager.GetSubscriptionsForDate(lastReward).FirstOrDefault();
                    ISubscriptionInfo currentSubscriptionReward = StoreManager.GetSubscriptionsForDate(checkedDate).FirstOrDefault();

                    if (!StoreManager.HasAnyActiveSubscription)
                    {
                        checkedDate = StoreManager.GetLastSubscription().ExpirationDate;
                    }

                    if (lastSubscriptionReward != null &&
                        currentSubscriptionReward != null)
                    {
                        if (lastSubscriptionReward.ProductId == currentSubscriptionReward.ProductId)
                        {
                            span = checkedDate - lastReward;
                        }
                        else
                        {
                            TimeSpan lastSpan = lastSubscriptionReward.ExpirationDate - lastReward;
                            TimeSpan currentSpan = checkedDate - currentSubscriptionReward.PurchaseDate;
                            span = lastSpan + currentSpan;
                        }
                    }
                }
                else
                {
                    ISubscriptionInfo result = StoreManager.GetLastSubscription();

                    if (!string.IsNullOrEmpty(result.ProductId))
                    {
                        LastSubscriptionRewardDate = result.PurchaseDate;
                        span = checkedDate - LastSubscriptionRewardDate;

                        return (StoreManager.IsSandboxEnvironment ? (span.Minutes / 2) : (span.Days)) + 1;
                    }
                }

                return StoreManager.IsSandboxEnvironment ? (span.Minutes / 2) : (span.Days);
            }
        }

        
        private DateTime LastSubscriptionRewardDate
        {
            get
            {
                return CustomPlayerPrefs.GetDateTime(SubscriptionRewardDateKey, DateTime.MinValue);
            }
            set
            {
                CustomPlayerPrefs.SetDateTime(SubscriptionRewardDateKey, value);
            }
        }

        #endregion



        #region Methods

        public SubscriptionRewardManager(IStoreManager storeManager)
        {
            StoreManager = storeManager;
        }


        public void ClaimReward()
        {
            DateTime checkedDate = StoreUtilities.VerifiedDateTime;
            ISubscriptionInfo currentSubscriptionReward = StoreManager.GetSubscriptionsForDate(checkedDate).FirstOrDefault();
            TimeSpan span = checkedDate - LastSubscriptionRewardDate;


            if (currentSubscriptionReward != null && !string.IsNullOrEmpty(currentSubscriptionReward.ProductId))
            {
                LastSubscriptionRewardDate = StoreManager.IsSandboxEnvironment ?
                    LastSubscriptionRewardDate.AddMinutes((span.Minutes / 2) * 2.0f) :
                    LastSubscriptionRewardDate.AddDays(span.Days);
            }
            else
            {
                LastSubscriptionRewardDate = checkedDate;
            }
        }

        #endregion
    }
}
