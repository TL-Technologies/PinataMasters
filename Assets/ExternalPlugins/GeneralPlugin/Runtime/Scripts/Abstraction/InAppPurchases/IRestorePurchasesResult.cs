using System.Collections.Generic;


namespace Modules.General.Abstraction.InAppPurchase
{
    public interface IRestorePurchasesResult
    {
        RestorePurchasesResultCode ResultCode { get; }
        string Message { get; }
        HashSet<IStoreItem> StoreItems { get; }
        bool IsSucceeded { get; }
    }
}
