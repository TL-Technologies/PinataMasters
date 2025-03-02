using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Collections.Generic;


namespace Modules.InAppPurchase
{
    internal class RestorePurchasesOperation
    {
        private Action<RestorePurchasesResult> callback;

        public HashSet<IStoreItem> StoreItems { get; }


        public RestorePurchasesOperation(Action<RestorePurchasesResult> callback)
        {
            this.callback = callback;
            StoreItems = new HashSet<IStoreItem>();
        }


        public void InvokeCallback(RestorePurchasesResult result)
        {
            callback?.Invoke(result);
        }
    }
}
