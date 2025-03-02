using System;


namespace Modules.General.Abstraction
{
    public interface IAdvertisingNecessaryInfo
    {
        bool IsSubscriptionActive { get; }
        bool IsNoAdsActive { get; }
        event Action OnPurchasesListUpdate;

        bool IsPersonalDataDeleted { get; }
        event Action OnPersonalDataDeletingDetect;
        
        int CurrentPlayerLevel { get; }
        event Action<int> OnPlayerLevelUpdate;
    }
}
