using Modules.General.Abstraction.InAppPurchase;
using System.Collections.Generic;


namespace Modules.InAppPurchase
{
    public class RestorePurchasesResult : IRestorePurchasesResult
    {
        public RestorePurchasesResultCode ResultCode { get; }

        public string Message { get; }

        public HashSet<IStoreItem> StoreItems { get; internal set; }

        public bool IsSucceeded => ResultCode == RestorePurchasesResultCode.Ok;


        internal RestorePurchasesResult(RestorePurchasesResultCode resultCode, string message = null)
        {
            ResultCode = resultCode;
            Message = message;
            StoreItems = new HashSet<IStoreItem>();
        }
    }
}
