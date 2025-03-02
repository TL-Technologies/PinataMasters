using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    [Serializable]
    internal class StoreItemSettings
    {
        public string id = null;
        public List<StoreIdInfo> storeSpecificIds = null;
        public int priceTier = 0;
        public ProductType itemType = ProductType.Consumable;
        public SubscriptionType subscriptionType = SubscriptionType.None;
    }
}
