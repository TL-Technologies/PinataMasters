using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.General.Utilities;
using Modules.Hive.Ioc;
using Modules.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;


namespace Modules.InAppPurchase
{
    [InitQueueService(
        -5000,
        typeof(IStoreManager),
        typeof(IStoreSettings),
        typeof(IAdvertisingIdentifier))]
    public sealed partial class StoreManager : IStoreManager, IStoreListener, IInitializableService, IPurchasingModuleProvider
    {
        #region Fields

        internal static readonly Dictionary<AppStore, string> SupportedStoreNames = new Dictionary<AppStore, string>
        {
            { AppStore.AmazonAppStore, AmazonApps.Name },
            { AppStore.AppleAppStore, AppleAppStore.Name },
            { AppStore.GooglePlay, GooglePlay.Name },
            { AppStore.HuaweiAppGallery, HuaweiAppGallery.Name },
            { AppStore.fake, "fake" } // This store is used in the Unity Editor
        };
        
        public event Action<IStoreItem> ItemDataReceived;
        public event Func<IPurchaseItemResult, bool> PurchaseComplete;
        public event Action<IRestorePurchasesResult> RestorePurchasesComplete;
        
        private bool isInitialized = false;
        private CancellationTokenSource cancellationTokenSource = null;
        private PurchaseItemOperation purchaseItemOperation = null;
        private RestorePurchasesOperation restorePurchasesOperation = null;
        
        private List<StoreItem> storeItems = null;
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;
        private StoreAnalyticsHelper storeAnalyticsHelper;
        private PurchaseValidator purchaseValidator;
        private IPurchaseHistoryHelper purchaseHistoryHelper;
        private ConfigurationBuilder configurationBuilder;
        
        private Dictionary<string, string> appleIntroductoryPriceDictionary;

        private StoreItemProvider storeItemProvider;

        #endregion



        #region Properties

        private PurchaseValidator PurchaseValidator => purchaseValidator ?? (purchaseValidator = new PurchaseValidator());

        
        public IPurchasingModule CustomPurchasingModule { get; set; }
        

        public bool IsInitialized => isInitialized;


        private StoreItemProvider StoreItemProvider
        {
            get
            {
                if (storeItemProvider == null)
                {
                    if (Application.isEditor)
                    {
                        storeItemProvider = new StoreItemEditorProvider(storeItems);
                    }
                    else
                    {
                        storeItemProvider = new StoreItemProvider(storeItems);
                    }
                }

                return storeItemProvider;
            }
        }

        #endregion



        #region IInitializableService

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            string errorMessage = Initialize(
                container.GetService<IStoreSettings>(),
                container.GetService<IAdvertisingIdentifier>(),
                container.GetService<IPurchaseAnalytics>(),
                container.GetService<IPurchaseAnalyticsParameters>());
            
            if (string.IsNullOrEmpty(errorMessage))
            {
                onCompleteCallback?.Invoke();
            }
            else
            {
                CustomDebug.LogError(errorMessage);
                onErrorCallback?.Invoke(this, InitializationStatus.Failed);
            }
        }
        
        #endregion



        #region Initialization

        private bool IsNativeStoreManagerInitialized =>
            storeController != null && extensionProvider != null;
        
        
        /// <summary>
        /// Performs initialization.
        /// Do it before calling any other methods and properties of the class.
        /// </summary>
        public string Initialize(
            IStoreSettings storeSettingsService,
            IAdvertisingIdentifier advertisingIdentifierService,
            IPurchaseAnalytics purchaseAnalytics = null,
            IPurchaseAnalyticsParameters purchaseAnalyticsParameters = null)
        {
            string errorMessage = string.Empty;
            if (isInitialized)
            {
                errorMessage = $"StoreManager: {nameof(StoreManager)} is already initialized.";
            }
            else if (storeSettingsService == null || advertisingIdentifierService == null)
            {
                errorMessage = $"StoreManager: can't initialize StoreManager because {typeof(IStoreSettings)} == null " +
                    $"and/or {typeof(IAdvertisingIdentifier)} == null!";
            }
            else
            {
                cancellationTokenSource = new UnityCancellationTokenSource();
                storeAnalyticsHelper = new StoreAnalyticsHelper(
                    storeSettingsService,
                    advertisingIdentifierService,
                    purchaseAnalytics,
                    purchaseAnalyticsParameters);
                
                configurationBuilder = ConfigurationBuilder.Instance(CustomPurchasingModule ?? StoreUtilities.PurchasingModule);
                
                storeItems = storeSettingsService.GetStoreItems(this).Cast<StoreItem>().ToList();

                foreach (StoreItem item in storeItems)
                {
                    configurationBuilder.AddProduct(
                        item.Identifier,
                        item.ItemType,
                        new IDs
                        {
                            { item.ProductId, SupportedStoreNames[StoreUtilities.StoreType] }
                        });
                
                    if (item.HasParentProduct)
                    {
                        configurationBuilder.AddProduct(
                            item.ParentProductId,
                            item.ItemType,
                            new IDs
                            {
                                { item.ParentProductId, SupportedStoreNames[StoreUtilities.StoreType] }
                            });
                    }
                }
                
                if (ReachabilityHandler.Instance.NetworkStatus == NetworkStatus.NotReachable)
                {
                    ReachabilityHandler.Instance.NetworkReachabilityStatusChanged += 
                        ReachabilityHandler_OnNetworkReachabilityStatusChanged;

                    CustomDebug.Log("StoreManager: currently offline, wait initializing UnityPurchasing until becomes online");
                }
                else
                {
                    UnityPurchasing.Initialize(this, configurationBuilder);
                }
            }
            
            return errorMessage;
        }


        [Conditional("DEBUG")]
        private void AssertManagerInitialized()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException($"{nameof(StoreManager)} is not initialized yet." );
            }
        }

        #endregion
        
        
        
        #region Purchase item
        
        /// <summary>
        /// Performs an item purchasing operation.
        /// </summary>
        /// <param name="item">The item to purchase.</param>
        /// <param name="callback">The operation callback with completion data.</param>
        public void PurchaseItem(IStoreItem item, Func<IPurchaseItemResult, bool> callback = null)
        {
            CustomDebug.Log($"StoreManager: purchase is initiated {item.ProductId}");
            
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            AssertManagerInitialized();
            if (!IsNativeStoreManagerInitialized)
            {
                string message = $"StoreManager: cannot start purchase operation for {item.ProductId} " +
                    "because native store manager hasn't been initialized yet.";
                CustomDebug.Log(message);
                callback?.Invoke(new PurchaseItemResult(item, PurchaseItemResultCode.InvalidState, message));
                return;
            }
            
            if (!CanStartExclusiveOperation)
            {
                string message = $"StoreManager: cannot start purchase operation for {item.ProductId} while another is in progress.";
                CustomDebug.Log(message);
                callback?.Invoke(new PurchaseItemResult(item, PurchaseItemResultCode.InvalidState, message));
                return;
            }

            var storeSettings = Services.GetService<IStoreSettings>();
            Product product;
            
            if (storeSettings.ShouldPurchaseParentProduct && item.HasParentProduct)
            {
                product = storeController.products.WithID(item.ParentProductId);
            }
            else
            {
                product = storeController.products.WithID(item.Identifier);
            }
            
            if (product != null && product.availableToPurchase)
            {
                purchaseItemOperation = new PurchaseItemOperation(item, callback);
                storeController.InitiatePurchase(product, storeAnalyticsHelper.DeveloperPayload);
            }
            else
            {
                CustomDebug.Log($"StoreManager: purchasing product {item.Identifier} failed: it's either not found or not available for purchase.");
            }
        }
        
        
        /// <summary>
        /// Performs an item purchasing operation.
        /// </summary>
        /// <param name="identifier">An identifier of the item to purchase.</param>
        /// <param name="callback">The operation callback with completion data.</param>
        public void PurchaseItem(string identifier, Func<IPurchaseItemResult, bool> callback = null) =>
            PurchaseItem(GetStoreItem(identifier), callback);
        
        #endregion



        #region Restore purchases
        
        /// <summary>
        /// Gets whether restore purchases is implemented for current platform.
        /// </summary>
        public bool IsRestorePurchasesImplemented => SupportedStoreNames.Keys.Contains(StoreUtilities.StoreType);
        

        /// <summary>
        /// Performs purchases restoration operation.
        /// </summary>
        /// <param name="callback">The operation callback with completion data.</param>
        public void RestorePurchases(Action<IRestorePurchasesResult> callback = null)
        {
            CustomDebug.Log($"StoreManager: restore purchases is initiated");
            
            AssertManagerInitialized();
            
            if (!IsNativeStoreManagerInitialized)
            {
                string message = "StoreManager: cannot start restore purchases operation because native store manager hasn't been initialized yet.";
                CustomDebug.Log(message);
                callback?.Invoke(new RestorePurchasesResult(RestorePurchasesResultCode.InvalidState, message));
                return;
            }

            if (!CanStartExclusiveOperation)
            {
                string message = "StoreManager: cannot start restore purchases operation while another is in progress.";
                CustomDebug.Log(message);
                callback?.Invoke(new RestorePurchasesResult(RestorePurchasesResultCode.InvalidState, message));
                return;
            }

            AppStore storeType = StoreUtilities.StoreType;
            if (IsRestorePurchasesImplemented)
            {
                restorePurchasesOperation = new RestorePurchasesOperation(callback);

                switch (storeType)
                {
                    case AppStore.AmazonAppStore: 
                    case AppStore.HuaweiAppGallery:
                        // Unity IAP plugin automatically restores the Amazon/Huawei purchases on initialize
                        OnRestoreComplete(true);
                        break;
                    case AppStore.AppleAppStore:
                        extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestoreComplete);
                        break;
                    case AppStore.GooglePlay:
                        extensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(OnRestoreComplete);
                        break;
                    case AppStore.fake:
                        DeferredInvokeAsync(SimulateRestorePurchasesProcess, true);
                        break;
                    default:
                        throw new NotImplementedException($"Restore purchases is not implemented for store {storeType}!");
                }
            }
            else
            {
                throw new NotImplementedException($"Restore purchases is not implemented for store {storeType}!");
            }
            
            
            void SimulateRestorePurchasesProcess(bool success)
            {
                List<StoreItem> purchasedItems = StoreItemProvider.GetStoreItems();

                foreach (StoreItem item in purchasedItems)
                {
                    PurchaseEventArgs eventArgs = (PurchaseEventArgs)Activator.CreateInstance(
                        typeof(PurchaseEventArgs),
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                        null,
                        new object[] { item.Product },
                        null);
                    ProcessPurchase(eventArgs);
                }
                OnRestoreComplete(success);
            }
            
            
            async void DeferredInvokeAsync<T>(Action<T> action, T v1, int delay = -1)
            {
                if (action == null)
                {
                    return;
                }
                if (delay <= 0)
                {
                    delay = UnityEngine.Random.Range(300, 1000);
                }

                await Task.Delay(delay, cancellationTokenSource.Token);

                action(v1);
            }
        }
        
        
        private void OnRestoreComplete(bool succeeded)
        {
            CustomDebug.Log($"StoreManager: restore purchases is completed {succeeded}, {restorePurchasesOperation}");
            
            if (restorePurchasesOperation == null)
            {
                return;
            }

            // Create operation result
            RestorePurchasesResultCode resultCode = succeeded ?
                RestorePurchasesResultCode.Ok :
                RestorePurchasesResultCode.Failed;
            RestorePurchasesResult result = new RestorePurchasesResult(resultCode)
            {
                StoreItems = restorePurchasesOperation.StoreItems
            };
            
            // Reset operation
            var cachedOperation = restorePurchasesOperation;
            restorePurchasesOperation = null;

            // Perform all event actions
            // result.StoreItem.InvokePurchaseRestored(...); // It's already called from OnPurchaseSuccessful
            RestorePurchasesComplete?.Invoke(result);
            cachedOperation.InvokeCallback(result);
        }

        #endregion



        #region IStoreListener

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            isInitialized = true;
            CustomDebug.Log("StoreManager: unity initialization succeeded");
            
            storeController = controller;
            extensionProvider = extensions;
            #if UNITY_IOS
                appleIntroductoryPriceDictionary = extensionProvider.GetExtension<IAppleExtensions>().GetIntroductoryPriceDictionary();
            #endif
            purchaseHistoryHelper = StoreUtilities.CreateHistoryHelper(extensionProvider);

            foreach (StoreItem item in storeItems)
            {
                Product product = storeController.products.WithID(item.Identifier);

                if (item.HasParentProduct)
                {
                    item.InvokeDataReceived(product, GetSubscriptionInfo(storeController.products.WithID(item.ParentProductId)), storeController.products.WithID(item.ParentProductId));
                }
                else
                {
                    item.InvokeDataReceived(product, GetSubscriptionInfo(product));
                }
                ItemDataReceived?.Invoke(item);
            }
        }
        
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            CustomDebug.LogError($"[StoreManager - OnInitializeFailed] {error}");
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            CustomDebug.Log($"[StoreManager - ProcessPurchase] \n{JsonConvert.SerializeObject(purchaseEvent)}");

            Product purchasedProduct = purchaseEvent.purchasedProduct;
            
            PurchaseItemResult result;
            
            // Keep shop item if it's a restore purchases operation 
            if (restorePurchasesOperation != null)
            {
                StoreItem item = GetStoreItem(purchasedProduct.definition.id) as StoreItem;
                if (item == null)
                {
                    return PurchaseProcessingResult.Complete;
                }

                // Add item to result and invoke event
                result = new PurchaseItemResult(
                    item,
                    purchasedProduct,
                    PurchaseTransactionState.Restored,
                    PurchaseValidator.GetPurchaseValidationState(item.Product));
                
                if (result.IsSucceeded)
                {
                    StoreItemProvider.SetStoreItemPurchasedInternal(item);
                    restorePurchasesOperation.StoreItems.Add(item);
                }
                
                if (item.HasParentProduct)
                {
                    item.InvokePurchaseRestored(result, GetSubscriptionInfo(storeController.products.WithID(item.ParentProductId)));
                }
                else
                {
                    item.InvokePurchaseRestored(result, GetSubscriptionInfo(purchasedProduct));
                }
                
                return PurchaseProcessingResult.Complete;
            }
            
            // PurchaseItemOperation == null in case of automatics products restore on app launch and
            // in case of deferred purchases, which currently are not supported
            PurchaseTransactionState transactionState = purchaseItemOperation == null ?
                PurchaseTransactionState.RePurchased :
                PurchaseTransactionState.Purchased;
            // Create operation result
            result = new PurchaseItemResult(
                GetStoreItem(purchasedProduct.definition.id),
                purchasedProduct,
                transactionState,
                PurchaseValidator.GetPurchaseValidationState(purchasedProduct));
            if (result.IsSucceeded)
            {
                StoreItemProvider.SetStoreItemPurchasedInternal(result.StoreItem);
            }

            // Complete operation
            return CompletePurchaseOperation(((StoreItem) result.StoreItem).Product, result);
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            CustomDebug.Log($"[StoreManager - OnPurchaseFailed] product {product.metadata.localizedTitle}, reason {failureReason}");   
            // Invalidate shop items if it's a restore purchases operation 
            if (restorePurchasesOperation != null)
            {
                StoreItem item = GetStoreItem(product.definition.id) as StoreItem;

                // Invoke event for item
                if (item != null)
                {
                    if (item.HasParentProduct)
                    {
                        item.InvokePurchaseRestored(new PurchaseItemResult(item, PurchaseItemResultCode.Failed), GetSubscriptionInfo(storeController.products.WithID(item.ParentProductId)));
                    }
                    else
                    {
                        item.InvokePurchaseRestored(new PurchaseItemResult(item, PurchaseItemResultCode.Failed), GetSubscriptionInfo(product));
                    }
                }
                
                return;
            }
            
            // Create operation result
            PurchaseItemResult result = new PurchaseItemResult(
                GetStoreItem(product.definition.id),
                PurchaseItemResultCode.Failed,
                failureReason.ToString());

            // Complete operation
            CompletePurchaseOperation(((StoreItem) result.StoreItem).Product, result);
        }

        #endregion



        #region Get info
        
        /// <summary>
        /// Returns a collection of registered store items.
        /// </summary>
        public List<IStoreItem> StoreItems => storeItems.Cast<IStoreItem>().ToList();
        
        
        /// <summary>
        /// Gets whether the store works in sandbox mode.
        /// </summary>
        public bool IsSandboxEnvironment => StoreUtilities.IsSandboxEnvironment;


        /// <summary>
        /// Returns an instance of store item with specified identifier or null if it does not exist.
        /// </summary>
        /// <param name="identifier">The identifier of store item.</param>
        /// <returns></returns>
        public IStoreItem GetStoreItem(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }
            
            StoreItem item = storeItems.Find(p => p != null && p.IsIdentifierEqualsTo(identifier));
            if (item == null)
            {
                item = storeItems.Find(p => p != null && p.IsProductIdEqualsTo(identifier));
                
                if (item == null)
                {
                    CustomDebug.LogError($"StoreManager: an item with id '{identifier}' is not registered.");
                }
            }

            return item;
        }
        
        
        /// <summary>
        /// Returns true if specified store item is a purchased non-consumable product or active subscription;
        /// otherwise - false.
        /// <para>
        /// KEEP IN MIND! This property only works with non-consumable products and subscriptions.
        /// </para>
        /// </summary>
        /// <param name="storeItem">The store item to check.</param>
        /// <returns></returns>
        public bool IsStoreItemPurchased(IStoreItem storeItem)
        {
            if (storeItem == null)
            {
                return false;
            }

            bool result = false;
            
            if (storeItem.IsNonConsumable)
            {
                result = IsStoreItemPurchasedInternal(storeItem);
            }
            else if (storeItem.IsSubscription)
            {
                result = GetActiveSubscriptions().Exists(item => item.IsProductIdEqualsTo(storeItem.ProductId));
            }

            return result;
        }
        
        
        /// <summary>
        /// Returns true if store item with specified identifier is a purchased non-consumable product or 
        /// active subscription; otherwise - false.
        /// </summary>
        /// <param name="identifier">Identifier of store item to check.</param>
        /// <returns></returns>
        public bool IsStoreItemPurchased(string identifier)
        {
            IStoreItem item = GetStoreItem(identifier);
            return IsStoreItemPurchased(item);
        }

        #endregion



        #region Methods
        
        public void ConfirmPendingPurchase(IPurchaseItemResult result)
        {
            storeController.ConfirmPendingPurchase(((StoreItem) result.StoreItem).Product);
        }
        
        
        private bool CanStartExclusiveOperation => 
            purchaseItemOperation == null && restorePurchasesOperation == null;
        
        
        private bool IsStoreItemPurchasedInternal(IStoreItem item) =>
            CustomPlayerPrefs.GetInt(item.ProductId, 0) != 0;
        
        
        private PurchaseProcessingResult CompletePurchaseOperation(Product product, PurchaseItemResult result)
        {
            CustomDebug.Log($"[StoreManager - CompletePurchaseOperation] product {product.definition.id}, result {result.ResultCode}," +
                            $" receipt : {product.receipt}");
            // Reset operation
            PurchaseItemOperation cachedOperation = purchaseItemOperation;
            purchaseItemOperation = null;

            ISubscriptionInfo subscriptionInfo = null;
            StoreItem item = GetStoreItem(product.definition.id) as StoreItem;

            if (item != null)
            {
                if (item.HasParentProduct)
                {
                    subscriptionInfo = GetSubscriptionInfo(storeController.products.WithID(item.ParentProductId));
                }
                else
                {
                    subscriptionInfo = GetSubscriptionInfo(product);
                } 
            }
            
            // Perform all event actions
            bool isRewardGranted = (result.StoreItem as StoreItem).InvokePurchaseComplete(
                result,
                subscriptionInfo);
            if (PurchaseComplete != null)
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (Func<IPurchaseItemResult, bool> func in PurchaseComplete.GetInvocationList())
                {
                    isRewardGranted |= func(result);
                }
            }
            isRewardGranted |= cachedOperation?.InvokeCallback(result) ?? result.StoreItem.IsSubscription;    
            
            // Send analytics event
            _ = storeAnalyticsHelper.SendPurchaseCompleteEventAsync(result);
            
            return isRewardGranted ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }

        #endregion



        #region  Events handlers

        private void ReachabilityHandler_OnNetworkReachabilityStatusChanged(NetworkStatus networkStatus)
        {
            CustomDebug.Log($"[StoreManager - ReachabilityHandler_OnNetworkReachabilityStatusChanged] isInitialized {isInitialized}," +
                            $" networkStatus {networkStatus}");
            if (isInitialized || networkStatus == NetworkStatus.NotReachable)
            {
                return;
            }
            
            ReachabilityHandler.Instance.NetworkReachabilityStatusChanged -=
                ReachabilityHandler_OnNetworkReachabilityStatusChanged;
            
            UnityPurchasing.Initialize(this, configurationBuilder);
        }

        #endregion
    }
}