using Modules.General.Abstraction.InAppPurchase;
using Modules.InAppPurchase.InternalRestore;
using System.Collections.Generic;
using System.Linq;


namespace Modules.InAppPurchase
{
    internal class StoreItemEditorProvider : StoreItemProvider
    {
        #region Fields

        private InternalRestorePrefs internalRestore;
        
        #endregion


        #region Properties

        private InternalRestorePrefs InternalRestore => 
            internalRestore ?? (internalRestore = InternalRestorePrefs.Open());

        #endregion

        
        #region Initialization

        public StoreItemEditorProvider(List<StoreItem> storeItems) : base(storeItems) { }

        #endregion

        
        #region Methods

        public override List<StoreItem> GetStoreItems()
        {
            List<string> restoreItemIds = InternalRestore.GetStoreItems();
            return storeItems.Where(item => restoreItemIds.Contains(item.ProductId)).ToList();
        }


        public override void SetStoreItemPurchasedInternal(IStoreItem item)
        {
            // Keep all purchased non-consumable products (in editor mode also keep subscriptions)
            if (!item.IsNonConsumable &&
                !item.IsSubscription)
            {
                return;
            }

            base.SetStoreItemPurchasedInternal(item);

            InternalRestore.AddStoreItem(item.ProductId);
            InternalRestore.Save();
        }

        #endregion
    }
}
