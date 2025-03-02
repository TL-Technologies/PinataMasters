using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Reflection;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


namespace Modules.InAppPurchase
{
    internal class PurchaseValidator
    {
        private const string GooglePlayTangleTypeName = "UnityEngine.Purchasing.Security.GooglePlayTangle";
        private const string AppleTangleTypeName = "UnityEngine.Purchasing.Security.AppleTangle";
        
        
        private CrossPlatformValidator validator;
        
        
        private bool IsValidationSupported
        {
            get
            {
                AppStore storeType = StoreUtilities.StoreType;
                
                return storeType == AppStore.GooglePlay ||
                    storeType == AppStore.AppleAppStore ||
                    storeType == AppStore.MacAppStore;
            }
        }
            
            
        public PurchaseValidator()
        {
            if (IsValidationSupported)
            {
                Type googlePlayTangleType = null;
                Type appleTangleType = null;
                
                // Finding project specific classes for a verification 
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (googlePlayTangleType == null)
                    {
                        googlePlayTangleType = assembly.GetType(GooglePlayTangleTypeName);
                    }
                    if (appleTangleType == null)
                    {
                        appleTangleType = assembly.GetType(AppleTangleTypeName);
                    }
                }
                // Fallback to default classes
                if (googlePlayTangleType == null)
                {
                    Debug.LogError("Can't find the class with data for a GooglePlay purchases verification!");
                    googlePlayTangleType = typeof(GooglePlayTangleDummy);
                }
                if (appleTangleType == null)
                {
                    Debug.LogError("Can't find the class with data for an Apple purchases verification!");
                    appleTangleType = typeof(AppleTangleDummy);
                }
                
                Func<byte[]> googlePlayDataMethod = ReflectionHelper.CreateDelegateToMethod<Func<byte[]>>(
                    googlePlayTangleType,
                    "Data",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    true);
                Func<byte[]> appleDataMethod = ReflectionHelper.CreateDelegateToMethod<Func<byte[]>>(
                    appleTangleType,
                    "Data",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    true);

                validator = new CrossPlatformValidator(
                    googlePlayDataMethod.Invoke(),
                    appleDataMethod.Invoke(),
                    Application.identifier);
            }
        }
        
        
        public PurchaseValidationState GetPurchaseValidationState(Product product)
        {
            if (!IsValidationSupported)
            {
                return PurchaseValidationState.Valid;
            }

            try
            {
                PurchaseValidationState validationState = PurchaseValidationState.Invalid;
                
                IPurchaseReceipt[] result = validator.Validate(product.receipt);
                foreach (IPurchaseReceipt purchaseReceipt in result)
                {
                    if (purchaseReceipt.productID.Equals(product.definition.storeSpecificId))
                    {
                        validationState = PurchaseValidationState.Valid;
                        break;
                    }
                }

                return validationState;

            }
            catch (IAPSecurityException exception)
            {
                Debug.LogError($"Invalid receipt!\n{exception}");
                return PurchaseValidationState.Invalid;
            }
        }
    }
}
