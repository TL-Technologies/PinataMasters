using Modules.General.Abstraction.InAppPurchase;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    internal class DummyPurchaseHistoryHelper : IPurchaseHistoryHelper
    {
        public void Initialize(IExtensionProvider extensionProvider) { }


        public bool WasItemPurchased(IStoreItem storeItem) => false;
    }
}
