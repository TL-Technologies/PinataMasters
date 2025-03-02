using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using System.Collections.Generic;
using System.Linq;


namespace Modules.InAppPurchase
{
    internal class StoreItemProvider
    {
        #region Fields

        protected List<StoreItem> storeItems;

        #endregion


        #region Initialization

        public StoreItemProvider(List<StoreItem> storeItems)
        {
            this.storeItems = storeItems;
        }

        #endregion


        #region Methods

        public virtual List<StoreItem> GetStoreItems()
        {
            return storeItems.Where(item => item.IsPurchased).ToList();
        }


        public virtual void SetStoreItemPurchasedInternal(IStoreItem item)
        {
            // Keep all purchased non-consumable products
            if (!item.IsNonConsumable)
            {
                return;
            }

            CustomPlayerPrefs.SetInt(item.ProductId, 1);
        }

        #endregion
    }
}
