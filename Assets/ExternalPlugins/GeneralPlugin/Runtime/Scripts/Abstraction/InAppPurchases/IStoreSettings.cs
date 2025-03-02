using Modules.General.Abstraction.InAppPurchase;
using System.Collections.Generic;


namespace Modules.InAppPurchase
{
    public interface IStoreSettings
    {
        string AppStoreAppId { get; }
        string GooglePlayAppPublicKey { get; }
        bool ShouldPurchaseParentProduct { get; set; }
        
        List<IStoreItem> GetStoreItems(IStoreManager storeManager);
    }
}
