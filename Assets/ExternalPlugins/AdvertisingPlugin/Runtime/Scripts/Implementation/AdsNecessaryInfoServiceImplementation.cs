using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Ioc;
using System;


namespace Modules.Advertising
{
    public class AdsNecessaryInfoServiceImplementation : IAdvertisingNecessaryInfo
    {
        #region Fields

        public event Action OnPurchasesListUpdate;
        public event Action OnPersonalDataDeletingDetect;
        public event Action<int> OnPlayerLevelUpdate;

        private IPrivacyManager privacyManager;
        private IStatisticsManager statisticsManager;
        private IStoreManager storeManager;

        private readonly string noAdsIdentifier;

        #endregion



        #region Properties

        public bool IsSubscriptionActive => (StoreManager?.HasAnyActiveSubscription) ?? (false);


        public bool IsNoAdsActive => (StoreManager?.IsStoreItemPurchased(noAdsIdentifier)) ?? (false);


        public bool IsPersonalDataDeleted => (PrivacyManager?.WasPersonalDataDeleted) ?? (false);


        public int CurrentPlayerLevel => (StatisticsManager?.CurrentLevelNumber) ?? (-1);


        private IPrivacyManager PrivacyManager
        {
            get
            {
                if (privacyManager == null)
                {
                    privacyManager = Services.PrivacyManager;
                    if (privacyManager != null)
                    {
                        EventDispatcher.Subscribe<PrivacyPersonalDataDeletingDetected>(detected =>
                            PrivacyManager_OnPersonalDataDeletingDetect());
                    }
                }

                return privacyManager;
            }
        }


        private IStatisticsManager StatisticsManager
        {
            get
            {
                if (statisticsManager == null)
                {
                    statisticsManager = Services.StatisticsManager;
                    if (statisticsManager != null)
                    {
                        statisticsManager.OnCurrentLevelChanged += StatisticsManager_OnCurrentLevelChanged;
                    }
                }

                return statisticsManager;
            }
        }


        private IStoreManager StoreManager
        {
            get
            {
                if (storeManager == null)
                {
                    storeManager = Services.GetService<IStoreManager>();
                    if (storeManager != null)
                    {
                        storeManager.RestorePurchasesComplete += StoreManager_OnRestorePurchasesComplete;
                        storeManager.PurchaseComplete += StoreManager_OnPurchaseComplete;
                    }
                }

                return storeManager;
            }
        }

        #endregion



        #region Class lifecycle

        public AdsNecessaryInfoServiceImplementation(string noAdsIdentifier)
        {
            this.noAdsIdentifier = noAdsIdentifier;
        }

        #endregion



        #region Events handlers

        private void PrivacyManager_OnPersonalDataDeletingDetect()
        {
            OnPersonalDataDeletingDetect?.Invoke();
        }


        private bool StoreManager_OnPurchaseComplete(IPurchaseItemResult purchaseItemResult)
        {
            OnPurchasesListUpdate?.Invoke();
            return false;
        }


        private void StoreManager_OnRestorePurchasesComplete(IRestorePurchasesResult restorePurchasesResult)
        {
            OnPurchasesListUpdate?.Invoke();
        }


        private void StatisticsManager_OnCurrentLevelChanged(int levelNumber)
        {
            OnPlayerLevelUpdate?.Invoke(CurrentPlayerLevel);
        }

        #endregion
    }
}
