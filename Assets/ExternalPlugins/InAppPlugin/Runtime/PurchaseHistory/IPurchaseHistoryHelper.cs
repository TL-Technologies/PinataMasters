using Modules.General.Abstraction.InAppPurchase;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal interface IPurchaseHistoryHelper
    {
        void Initialize(IExtensionProvider extensionProvider);
        bool WasItemPurchased(IStoreItem storeItem);
    }
}
