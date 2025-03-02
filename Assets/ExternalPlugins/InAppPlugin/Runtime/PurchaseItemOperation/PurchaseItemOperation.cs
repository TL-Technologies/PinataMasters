using Modules.General.Abstraction.InAppPurchase;
using System;


namespace Modules.InAppPurchase
{
    internal class PurchaseItemOperation
    {
        private Func<IPurchaseItemResult, bool> callback;

        public IStoreItem StoreItem { get; }


        public PurchaseItemOperation(IStoreItem storeItem, Func<IPurchaseItemResult, bool> callback)
        {
            StoreItem = storeItem;
            this.callback = callback;
        }


        public bool InvokeCallback(IPurchaseItemResult result)
        {
            return callback?.Invoke(result) ?? false;
        }
    }
}
