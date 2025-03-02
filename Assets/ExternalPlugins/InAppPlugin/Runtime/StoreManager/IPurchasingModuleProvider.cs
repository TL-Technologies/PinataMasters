using UnityEngine.Purchasing.Extension;

namespace Modules.InAppPurchase
{
    public interface IPurchasingModuleProvider
    {
        IPurchasingModule CustomPurchasingModule { get; set; }
    }
}