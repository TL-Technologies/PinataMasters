using Modules.General.Abstraction.InAppPurchase;
using Modules.General.InitializationQueue;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.InAppPurchase
{
    [CreateAssetMenu(fileName = "StoreSettings")]
    [InitQueueService(
        -5010,
        typeof(IStoreSettings))]
    public class StoreSettings : ScriptableObject, IStoreSettings
    {
        [SerializeField] private string appStoreAppId = "";
        [SerializeField] private string googlePlayAppPublicKey = "";
        [SerializeField] private bool shouldPurchaseParentProduct;
        [SerializeField] private List<StoreItemSettings> storeItemsSettings;
        
        private List<IStoreItem> storeItems;


        public string AppStoreAppId => appStoreAppId;
        public string GooglePlayAppPublicKey => googlePlayAppPublicKey;

        public bool ShouldPurchaseParentProduct
        {
            get => shouldPurchaseParentProduct;
            set => shouldPurchaseParentProduct = value;
        }


        public List<IStoreItem> GetStoreItems(IStoreManager storeManager)
        {
            if (storeItems == null)
            {
                storeItems = new List<IStoreItem>(storeItemsSettings.Count);
                foreach (StoreItemSettings settings in storeItemsSettings)
                {
                    storeItems.Add(new StoreItem(settings, storeManager));
                }
            }
                
            return storeItems;
        }
    }
}
