using Modules.General;
using Modules.General.Utilities;
using Modules.Hive;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif


namespace Modules.InAppPurchase
{
    internal static class StoreUtilities
    {
        #region Extern methods

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsPurchasesInSandboxEnvironment();
        #endif

        #endregion



        #region Fields

        private static StandardPurchasingModule purchasingModule = null;

        #endregion



        #region Properties

        public static AppStore StoreType
        {
            get
            {
                if (Application.isEditor)
                {
                    return AppStore.fake;
                }

                return AndroidAppStoreType == AppStore.HuaweiAppGallery
                    ? AppStore.HuaweiAppGallery
                    : PurchasingModule.appStore;
            }
        }


        public static DateTime VerifiedDateTime => DateTime.UtcNow;
        
        
        public static AppStore AndroidAppStoreType
        {
            get
            {
                AndroidTarget androidTarget = PlatformInfo.AndroidTarget;

                switch (androidTarget)
                {
                    case AndroidTarget.Amazon:
                        return AppStore.AmazonAppStore;
                    case AndroidTarget.GooglePlay:
                        return AppStore.GooglePlay;
                    case AndroidTarget.Samsung:
                        return AppStore.SamsungApps;
                    case AndroidTarget.Huawei:
                        return AppStore.HuaweiAppGallery;
                    default:
                        return AppStore.NotSpecified;
                }
            }
        }
        
        
        internal static StandardPurchasingModule PurchasingModule =>
            purchasingModule ?? (purchasingModule = StandardPurchasingModule.Instance(AndroidAppStoreType));

        #endregion



        #region Methods

        /// <summary>
        /// Returns a value of price in USD for specified tier
        /// </summary>
        /// <param name="tier">The number of tier.</param>
        /// <returns></returns>
        internal static float GetTierPrice(int tier)
        {
            // We don't use method ApplePriceTiers.ActualDollarsForAppleTier, because its existence
            // is not guaranteed on the non-Apple platforms
            if (tier <= 0)
            {
                return 0.0f;
            }

            if (tier <= 50)
            {
                return tier - 0.01f;
            }

            float price;
            switch (tier)
            {
                case 51: price = 55.0f; break;
                case 52: price = 60.0f; break;
                case 53: price = 65.0f; break;
                case 54: price = 70.0f; break;
                case 55: price = 75.0f; break;
                case 56: price = 80.0f; break;
                case 57: price = 85.0f; break;
                case 58: price = 90.0f; break;
                case 59: price = 95.0f; break;

                case 60: price = 100.0f; break;
                case 61: price = 110.0f; break;
                case 62: price = 120.0f; break;
                case 63: price = 125.0f; break;
                case 64: price = 130.0f; break;
                case 65: price = 140.0f; break;
                case 66: price = 150.0f; break;
                case 67: price = 160.0f; break;
                case 68: price = 170.0f; break;
                case 69: price = 175.0f; break;

                case 70: price = 180.0f; break;
                case 71: price = 190.0f; break;
                case 72: price = 200.0f; break;
                case 73: price = 210.0f; break;
                case 74: price = 220.0f; break;
                case 75: price = 230.0f; break;
                case 76: price = 240.0f; break;
                case 77: price = 250.0f; break;
                case 78: price = 300.0f; break;
                case 79: price = 350.0f; break;
                
                case 80: price = 400.0f; break;
                case 81: price = 450.0f; break;
                case 82: price = 500.0f; break;
                case 83: price = 600.0f; break;
                case 84: price = 700.0f; break;
                case 85: price = 800.0f; break;
                case 86: price = 900.0f; break;
                case 87: price = 1000.0f; break;

                default:
                    price = 1000.0f;
                    Debug.LogError("Failed to get price from tier.");
                    break;
            }

            return price - 0.01f;
        }
        
        
        internal static void CheckStoreItemSettings(StoreItemSettings settings)
        {
            foreach (StoreIdInfo idInfo in settings.storeSpecificIds)
            {
                if (!StoreManager.SupportedStoreNames.ContainsKey(idInfo.storeType))
                {
                    Debug.LogError($"Store type {idInfo.storeType} in {settings.id} is not supported!");
                }
            }
        }
        
        
        internal static bool IsSandboxEnvironment
        {
            get
            {
                #if UNITY_IOS && !UNITY_EDITOR
                    return IsPurchasesInSandboxEnvironment();
                #else
                    return Debug.isDebugBuild;
                #endif
            }
        }
        
        
        internal static IPurchaseHistoryHelper CreateHistoryHelper(IExtensionProvider extensionProvider)
        {
            IPurchaseHistoryHelper result;
            AppStore storeType = StoreType;
            
            if (storeType == AppStore.GooglePlay)
            {
                result = new GooglePlayPurchaseHistoryHelper();
            }
            else if (storeType == AppStore.AppleAppStore || storeType == AppStore.MacAppStore)
            {
                result = new ApplePurchaseHistoryHelper();
            }
            else
            {
                result = new DummyPurchaseHistoryHelper();
            }
            
            result.Initialize(extensionProvider);
            
            return result;
        }
        
        #endregion
    }
}
