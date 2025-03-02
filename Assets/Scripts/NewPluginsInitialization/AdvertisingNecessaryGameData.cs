using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Ioc;
using PinataMasters;
using System;


public class AdvertisingNecessaryGameData : IAdvertisingNecessaryInfo
{
    #region Fields

    public event Action OnPersonalDataDeletingDetect;

    public event Action OnPurchasesListUpdate;
    
    public event Action<int> OnPlayerLevelUpdate;

    private IStoreManager storeManager = null;

    #endregion
    
    
    
    #region Properties

    public bool IsSubscriptionActive => IAPs.IsAnySubscriptionActive;

    public bool IsNoAdsActive => false;
    
    public bool IsPersonalDataDeleted => Services.GetService<IPrivacyManager>().WasPersonalDataDeleted;

    public int CurrentPlayerLevel => (int)Player.TotalLevel;

    private IStoreManager StoreManager
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



    #region Methods

    public void InitListeners()
    {
        EventDispatcher.Subscribe<PrivacyPersonalDataDeletingDetected>(d => LLPrivacyManager_OnPersonalDataDeletingDetect());
        
        StoreManager.RestorePurchasesComplete += StoreManager_RestorePurchasesComplete;
        StoreManager.PurchaseComplete += StoreManager_PurchaseComplete;

        Player.OnLevelUp += PlayerOnOnLevelUp;
    }

    #endregion



    #region Events handlers

    private void LLPrivacyManager_OnPersonalDataDeletingDetect()
    {
        OnPersonalDataDeletingDetect?.Invoke();
    }


    private bool StoreManager_PurchaseComplete(IPurchaseItemResult result)
    {
        OnPurchasesListUpdate?.Invoke();
        return false;
    }


    private void StoreManager_RestorePurchasesComplete(IRestorePurchasesResult result)
    {
        OnPurchasesListUpdate?.Invoke();
    }


    private void PlayerOnOnLevelUp()
    {
        OnPlayerLevelUpdate?.Invoke((int)Player.TotalLevel);
    }

    #endregion
}