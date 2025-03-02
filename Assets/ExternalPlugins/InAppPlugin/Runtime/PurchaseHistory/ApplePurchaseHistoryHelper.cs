using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Reflection;
using System;
using System.Reflection;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


namespace Modules.InAppPurchase
{
    internal class ApplePurchaseHistoryHelper : IPurchaseHistoryHelper
    {
        private Func<string> encodedAppleReceiptGetter;
        private Func<string, AppleReceipt> appleReceiptGetter;
        
        
        public void Initialize(IExtensionProvider extensionProvider)
        {
            Type appStoreImplType = Type.GetType("UnityEngine.Purchasing.AppleStoreImpl, UnityEngine.Purchasing.Stores");
            IAppleExtensions appleExtension = extensionProvider.GetExtension<IAppleExtensions>();
            
            encodedAppleReceiptGetter = ReflectionHelper.CreateDelegateToPropertyGet<Func<string>>(
                appStoreImplType,
                appleExtension,
                "appReceipt",
                true);
            
            appleReceiptGetter = ReflectionHelper.CreateDelegateToMethod<Func<string, AppleReceipt>>(
                appStoreImplType,
                "getAppleReceiptFromBase64String",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                true,
                appleExtension);
        }


        public bool WasItemPurchased(IStoreItem storeItem)
        {
            bool result = false;
            AppleReceipt receipt = appleReceiptGetter(encodedAppleReceiptGetter());
            if (receipt != null)
            {
                foreach (AppleInAppPurchaseReceipt inAppPurchaseReceipt in receipt.inAppPurchaseReceipts)
                {
                    if (inAppPurchaseReceipt.productID == storeItem.ProductId)
                    {
                        result = true;
                        break;
                    }
                }
            }
            
            return result;
        }
    }
}
